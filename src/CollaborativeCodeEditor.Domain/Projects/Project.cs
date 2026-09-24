using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Domain.Projects.Events;

namespace CollaborativeCodeEditor.Domain.Projects;

public sealed class Project : AggregateRoot<ProjectId>
{
    public WorkspaceId WorkspaceId { get; }

    public ProjectName Name { get; private set; }

    private Project(
        ProjectId id,
        WorkspaceId workspaceId,
        ProjectName name)
        : base(id)
    {
        WorkspaceId = workspaceId;
        Name = name;
    }

    public static Project Create(
            ProjectId id,
            WorkspaceId workspaceId,
            ProjectName name)
    {
        if (id.IsEmpty)
            throw new ArgumentException(
                "Project ID cannot be empty.",
                nameof(id));

        if (workspaceId.IsEmpty)
            throw new ArgumentException(
                "Workspace ID cannot be empty.",
                nameof(workspaceId));

        var project = new Project(
            id,
            workspaceId,
            name);

        project.AddDomainEvent(
            new ProjectCreated(
                project.Id,
                project.WorkspaceId));

        return project;
    }


    public void Rename(ProjectName name)
    {
        if (Name == name)
            return;

        Name = name;
    }
}