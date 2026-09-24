using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Users.Events;

namespace CollaborativeCodeEditor.Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var userId = UserId.New();

        // Act
        var user = User.Create(
            userId,
            "Khaled");

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal("Khaled", user.DisplayName);
    }

    [Fact]
    public void Create_ShouldRaiseUserCreatedEvent()
    {
        // Arrange
        var userId = UserId.New();

        // Act
        var user = User.Create(
            userId,
            "Khaled");

        // Assert
        var domainEvent = Assert.Single(
            user.DomainEvents);

        Assert.IsType<UserCreated>(domainEvent);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.Empty,
                "Khaled"));
    }

    [Fact]
    public void Create_WithEmptyDisplayName_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.New(),
                ""));
    }

    [Fact]
    public void Create_WithWhitespaceDisplayName_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.New(),
                "   "));
    }

    [Fact]
    public void Create_WithDisplayNameLongerThan100Characters_ShouldThrow()
    {
        // Arrange
        var displayName = new string('A', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.New(),
                displayName));
    }

    [Fact]
    public void ChangeDisplayName_WithValidName_ShouldChangeName()
    {
        // Arrange
        var user = User.Create(
            UserId.New(),
            "Old Name");

        // Act
        user.ChangeDisplayName("New Name");

        // Assert
        Assert.Equal(
            "New Name",
            user.DisplayName);
    }

    [Fact]
    public void ChangeDisplayName_WithEmptyName_ShouldThrow()
    {
        // Arrange
        var user = User.Create(
            UserId.New(),
            "Khaled");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            user.ChangeDisplayName(""));
    }

    [Fact]
    public void Create_ShouldTrimDisplayName()
    {
        // Act
        var user = User.Create(
            UserId.New(),
            "  Khaled  ");

        // Assert
        Assert.Equal(
            "Khaled",
            user.DisplayName);
    }

    [Fact]
    public void ChangeDisplayName_ShouldTrimDisplayName()
    {
        // Arrange
        var user = User.Create(
            UserId.New(),
            "Khaled");

        // Act
        user.ChangeDisplayName(
            "  New Name  ");

        // Assert
        Assert.Equal(
            "New Name",
            user.DisplayName);
    }
}