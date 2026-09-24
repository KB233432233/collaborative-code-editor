namespace CollaborativeCodeEditor.Domain.Common;

public abstract record DomainEvent
{
    public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
}