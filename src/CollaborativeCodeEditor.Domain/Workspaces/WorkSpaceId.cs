namespace CollaborativeCodeEditor.Domain.Workspaces;

public readonly record struct WorkspaceId(Guid Value)
{
    public static WorkspaceId New()
        => new(Guid.NewGuid());

    public static WorkspaceId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}