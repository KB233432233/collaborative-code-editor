using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users.Events;

namespace CollaborativeCodeEditor.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    public DisplayName DisplayName { get; private set; }

    public Email Email { get; private set; }

    private User(
        UserId id,
        DisplayName displayName,
        Email email)
        : base(id)
    {
        DisplayName = displayName;
        Email = email;
    }

    public static User Create(
    UserId id,
    DisplayName displayName,
    Email email)
    {
        if (id.IsEmpty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(id));

        var user = new User(
            id,
            displayName,
            email);

        user.AddDomainEvent(
             new UserCreated(user.Id));

        return user;
    }

    public void ChangeDisplayName(DisplayName newDisplayName)
    {
        DisplayName = newDisplayName;
    }
}