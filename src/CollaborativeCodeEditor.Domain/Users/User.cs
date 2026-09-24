using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users.Events;

namespace CollaborativeCodeEditor.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    public string DisplayName { get; private set; }

    private User(
        UserId id,
        string displayName)
        : base(id)
    {
        DisplayName = displayName;
    }

    public static User Create(
    UserId id,
    string displayName)
    {
        if (id.IsEmpty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(id));

        ValidateDisplayName(displayName);

        var user = new User(
            id,
            displayName.Trim());

        user.AddDomainEvent(
             new UserCreated(user.Id));

        return user;
    }
    public void ChangeDisplayName(string displayName)
    {
        ValidateDisplayName(displayName);

        DisplayName = displayName.Trim();
    }

    private static void ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Display name cannot be empty.",
                nameof(displayName));

        if (displayName.Trim().Length > 100)
            throw new ArgumentException(
                "Display name cannot exceed 100 characters.",
                nameof(displayName));
    }
}