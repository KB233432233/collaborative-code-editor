using CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CollaborativeCodeEditor.Application.Tests.Users;

public sealed class RegisterNewUserTests : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public RegisterNewUserTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task RegisterNewUser_ShouldPersistUser()
    {
        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(
                    new UserId(Guid.NewGuid())));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var command =
            new RegisterNewUserCommand(
                "New User",
                "newuser@example.com");

        var result =
            await mediator.Send(command);

        result.IsSuccess.Should().BeTrue();

        var userId = result.Value;

        await using var verificationContext =
            new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(_database.ConnectionString)
                    .Options);

        var user =
            await verificationContext.Users
                .SingleOrDefaultAsync(
                    x => x.Id == new UserId(userId));

        user.Should().NotBeNull();

        user!.DisplayName
            .Should()
            .Be("New User");

        user!.Email
            .Should()
            .Be("newuser@example.com");
    }
}