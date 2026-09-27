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
        var displayName = DisplayName.Create("Khaled");
        var email = Email.Create("khaled@example.com");

        // Act
        var user = User.Create(
            userId,
            displayName,
            email);

        // Assert
        Assert.Equal(userId, user.Id);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(email, user.Email);
    }

    [Fact]
    public void Create_ShouldRaiseUserCreatedEvent()
    {
        // Arrange
        var userId = UserId.New();
        var displayName = DisplayName.Create("Khaled");
        var email = Email.Create("khaled@example.com");

        // Act
        var user = User.Create(
            userId,
            displayName,
            email);

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
                DisplayName.Create("Khaled"),
                Email.Create("khaled@example.com")));
    }

    [Fact]
    public void Create_WithEmptyDisplayName_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.New(),
                DisplayName.Create(""),
                Email.Create("khaled@example.com")));
    }

    [Fact]
    public void Create_WithWhitespaceDisplayName_ShouldThrow()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            User.Create(
                UserId.New(),
                DisplayName.Create("   "),
                Email.Create("khaled@example.com")));
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
                DisplayName.Create(displayName),
                Email.Create("khaled@example.com")));
    }

    [Fact]
    public void ChangeDisplayName_WithValidName_ShouldChangeName()
    {
        // Arrange
        var user = User.Create(
            UserId.New(),
            DisplayName.Create("Old Name"),
            Email.Create("khaled@example.com"));

        // Act
        var newDisplayName = DisplayName.Create("New Name");
        user.ChangeDisplayName(newDisplayName);

        // Assert
        Assert.Equal(
            newDisplayName,
            user.DisplayName);
    }

    [Fact]
    public void ChangeDisplayName_WithEmptyName_ShouldThrow()
    {
        // Arrange
        var user = User.Create(
            UserId.New(),
            DisplayName.Create("Khaled"),
            Email.Create("khaled@example.com"));

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            user.ChangeDisplayName(DisplayName.Create("")));
    }

    [Fact]
    public void Create_ShouldTrimDisplayName()
    {
        // Act
        var Name = DisplayName.Create("  Khaled  ");

        var user = User.Create(
            UserId.New(),
            Name,
            Email.Create("khaled@example.com"));

        // Assert
        Assert.Equal(
            "Khaled",
            user.DisplayName.Value);
    }

    [Fact]
    public void ChangeDisplayName_ShouldTrimDisplayName()
    {
        // Arrange
        var Name = DisplayName.Create("Khaled");

        var user = User.Create(
            UserId.New(),
            Name,
            Email.Create("khaled@example.com"));

        // Act
        var newName = DisplayName.Create("  New Name  ");

        user.ChangeDisplayName(
            newName);

        // Assert
        Assert.Equal(
            "New Name",
            user.DisplayName.Value);
    }
}