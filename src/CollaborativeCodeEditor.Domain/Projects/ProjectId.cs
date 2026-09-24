
namespace CollaborativeCodeEditor.Domain.Projects;

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New()
        => new(Guid.NewGuid());

    public static ProjectId Empty
        => new(Guid.Empty);

    public bool IsEmpty
        => Value == Guid.Empty;
}