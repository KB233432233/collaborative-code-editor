using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Workspaces.Events;

public sealed record MemberAddedToWorkspace(
    WorkspaceId WorkspaceId,
    UserId UserId,
    WorkspaceRole Role
) : DomainEvent;