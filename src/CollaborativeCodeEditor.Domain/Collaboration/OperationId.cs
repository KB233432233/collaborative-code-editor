namespace CollaborativeCodeEditor.Domain.Collaboration;

public readonly record struct OperationId(Guid Value)
{
    public static OperationId New()
        => new(Guid.NewGuid());

    public static OperationId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}