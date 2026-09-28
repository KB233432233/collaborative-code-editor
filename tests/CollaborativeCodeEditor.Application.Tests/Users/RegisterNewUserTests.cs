using CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using CollaborativeCodeEditor.Infrastructure.Authentication;


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
            serviceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();

        var identityUser =
    await userManager.FindByIdAsync(
        result.Value.ToString());

        identityUser.Should().NotBeNull();
        identityUser!.Email
            .Should()
            .Be("newuser@example.com");
    }

    [Fact]
    public async Task RegisterNewUser_Should_Persist_Domain_And_Identity_User()
    {
        // Arrange
        var currentUser = new FakeCurrentUser(new UserId(Guid.NewGuid()));

        var serviceProvider = TestServiceProvider.Create(
            _database.ConnectionString,
            currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        const string displayName = "Khaled";
        const string email = "khaled@example.com";
        const string password = "Password123!";

        // Act
        var result = await mediator.Send(
            new RegisterNewUserCommand(
                displayName,
                email,
                password));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var userId = result.Value;

        var dbContext =
            serviceProvider.GetRequiredService<AppDbContext>();

        var domainUser = await dbContext.Users
            .SingleAsync(x => x.Id == new UserId(userId));

        domainUser.DisplayName.Value
            .Should()
            .Be(displayName);

        domainUser.Email.Value
            .Should()
            .Be(email);

        var userManager =
            serviceProvider.GetRequiredService<
                UserManager<ApplicationIdentityUser>>();

        var identityUser =
            await userManager.FindByIdAsync(
                userId.ToString());

        identityUser.Should().NotBeNull();

        identityUser!.Id
            .Should()
            .Be(userId);

        identityUser.Email
            .Should()
            .Be(email);

        var passwordValid =
            await userManager.CheckPasswordAsync(
                identityUser,
                password);

        passwordValid.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterNewUser_WithDuplicateEmail_Should_Fail()
    {
        // Arrange
        var currentUser = new FakeCurrentUser(new UserId(Guid.NewGuid()));

        var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var command = new RegisterNewUserCommand(
            "Khaled",
            "khaled2@example.com",
            "Password123!");

        // First registration
        var firstResult =
            await mediator.Send(command);

        firstResult.IsSuccess.Should().BeTrue();

        // Act
        var secondResult =
            await mediator.Send(
                new RegisterNewUserCommand(
                    "Another User",
                    "khaled@example.com",
                    "Password456!"));

        // Assert
        secondResult.IsSuccess.Should().BeFalse();

        secondResult.Error.Should().NotBeNull();

        secondResult.Error!.Code
            .Should()
            .StartWith("Identity.");
    }

    [Fact]
    public async Task RegisterNewUser_WithInvalidPassword_Should_Fail()
    {
        // Arrange
        var currentUser = new FakeCurrentUser(new UserId(Guid.NewGuid()));

        var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result =
            await mediator.Send(
                new RegisterNewUserCommand(
                    "Khaled",
                    "khaled@example.com",
                    "password"));

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().NotBeNull();

        result.Error!.Code
            .Should()
            .StartWith("Identity.");
    }

    [Fact]
    public async Task RegisterNewUser_WithInvalidEmail_Should_Fail()
    {
        // Arrange
        var currentUser = new FakeCurrentUser(new UserId(Guid.NewGuid()));

        var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result =
            await mediator.Send(
                new RegisterNewUserCommand(
                    "Khaled",
                    "not-an-email",
                    "Password123!"));

        // Assert
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterNewUser_WithEmptyDisplayName_Should_Fail()
    {
        // Arrange
        var currentUser = new FakeCurrentUser(new UserId(Guid.NewGuid()));

        var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result =
            await mediator.Send(
                new RegisterNewUserCommand(
                    "",
                    "khaled@example.com",
                    "Password123!"));

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}