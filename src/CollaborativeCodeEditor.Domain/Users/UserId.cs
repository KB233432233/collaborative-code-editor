using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Users;

public readonly record struct UserId(Guid Value)
{
    public static UserId New()
        => new(Guid.NewGuid());

    public static UserId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}