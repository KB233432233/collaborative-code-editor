using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Users.Events;

public sealed record UserCreated(
    UserId UserId
) : DomainEvent;