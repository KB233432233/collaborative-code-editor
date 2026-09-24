using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Workspaces;

public static class WorkspaceErrors
{
    public static Error NotFound(Guid workspaceId)
        => new(
            "Workspace.NotFound",
            $"Workspace '{workspaceId}' was not found.");

    public static readonly Error AccessDenied =
        new(
            "Workspace.AccessDenied",
            "You do not have access to this workspace.");
}