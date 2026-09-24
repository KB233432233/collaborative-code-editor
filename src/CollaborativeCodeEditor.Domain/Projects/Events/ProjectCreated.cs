using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Workspaces;

namespace CollaborativeCodeEditor.Domain.Projects.Events;

public sealed record ProjectCreated(
    ProjectId ProjectId,
    WorkspaceId WorkspaceId
) : DomainEvent;