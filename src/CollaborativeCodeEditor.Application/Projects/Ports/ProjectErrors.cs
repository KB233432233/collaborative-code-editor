using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Projects;

public static class ProjectErrors
{
    public static Error WorkspaceNotFound(Guid workspaceId)
        => new(
            "Workspace.NotFound",
            $"Workspace '{workspaceId}' was not found.");

    public static readonly Error WorkspaceAccessDenied =
        new(
            "Workspace.AccessDenied",
            "You do not have access to this workspace.");

    public static Error NotFound(Guid projectId)
        => new(
            "Project.NotFound",
            $"Project '{projectId}' was not found.");
}