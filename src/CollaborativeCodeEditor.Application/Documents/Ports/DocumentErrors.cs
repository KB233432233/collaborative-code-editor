using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Documents;

public static class DocumentErrors
{
    public static Error ProjectNotFound(Guid projectId)
        => new(
            "Project.NotFound",
            $"Project '{projectId}' was not found.");

    public static readonly Error WorkspaceAccessDenied =
        new(
            "Workspace.AccessDenied",
            "You do not have access to this project's workspace.");

    public static Error NotFound(Guid documentId)
        => new(
            "Document.NotFound",
            $"Document '{documentId}' was not found.");

    public static readonly Error NameAlreadyExists =
        new(
            "Document.NameAlreadyExists",
            "A document with this name already exists in the project.");
}