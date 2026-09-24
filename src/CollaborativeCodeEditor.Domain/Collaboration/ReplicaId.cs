namespace CollaborativeCodeEditor.Domain.Collaboration;

public readonly record struct ReplicaId(Guid Value)
{
    public static ReplicaId New()
        => new(Guid.NewGuid());

    public static ReplicaId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}