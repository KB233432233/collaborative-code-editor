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
using FluentValidation;


namespace CollaborativeCodeEditor.Application.Tests.Users;

public sealed class RegisterNewUserTests : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public RegisterNewUserTests(TestDatabase database)
    {
        _database = database;
    }

    private static string UniqueEmail()
       => $"user.{Guid.NewGuid():N}@example.com";

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

        var displayName = "Khaled";
        var email = UniqueEmail();
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
        var email = UniqueEmail();

        var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var command = new RegisterNewUserCommand(
            "Khaled",
            email,
            "Password123!");

        // First registration
        var firstResult =
            await mediator.Send(command);

        firstResult.IsSuccess.Should().BeTrue(
        "the first registration of a fresh address must succeed");

        // Act
        var secondResult =
            await mediator.Send(
                new RegisterNewUserCommand(
                    "Another User",
                    email,
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
                    UniqueEmail(),
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
            () => mediator.Send(
                new RegisterNewUserCommand(
                    "Khaled",
                    "not-an-email",
                    "Password123!"));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>();
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
            () => mediator.Send(
                new RegisterNewUserCommand(
                    "",
                    UniqueEmail(),
                    "Password123!"));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>();
    }
}