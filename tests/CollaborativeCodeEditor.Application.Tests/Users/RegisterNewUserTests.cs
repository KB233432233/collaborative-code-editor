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
    public async Task RegisterNewUser_Should_Presist()
    {
        var userId = new UserId(Guid.NewGuid());

        // await using var serviceProvider =
        // TestServiceProvider.Create(
        //     _database.ConnectionString,
        //     currentUser);

        var command = new RegisterNewUserCommand("New User");
    }
}