using CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;


namespace CollaborativeCodeEditor.Application.Tests.Users;

public sealed class RegisterNewUserTests : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public RegisterNewUserTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task RegisterNewUser_Should_Persist()
    {
        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(new UserId(Guid.NewGuid())));

        var sender =
            serviceProvider.GetRequiredService<ISender>();

        var command =
            new RegisterNewUserCommand(
                "New User",
                "newuser@example.com",
                "Password123!");

        var result =
            await sender.Send(command);

        result.IsSuccess.Should().BeTrue();

        var dbContext =
            serviceProvider
                .GetRequiredService<AppDbContext>();

        var user =
            await dbContext.Users
                .FirstOrDefaultAsync(
                    x => x.Id == new UserId(result.Value));

        user.Should().NotBeNull();

        var userManager =
            serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var identityUser =
    await userManager.FindByIdAsync(
        result.Value.ToString());

        identityUser.Should().NotBeNull();
        identityUser!.Email
            .Should()
            .Be("newuser@example.com");
    }
}