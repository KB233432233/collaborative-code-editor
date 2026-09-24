namespace CollaborativeCodeEditor.Domain.Executions;

public readonly record struct ExecutionId(Guid Value)
{
    public static ExecutionId New()
        => new(Guid.NewGuid());

    public static ExecutionId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}