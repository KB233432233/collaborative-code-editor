using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Workspaces.Events;

public sealed record WorkspaceCreated(
    WorkspaceId WorkspaceId,
    UserId OwnerId
) : DomainEvent;