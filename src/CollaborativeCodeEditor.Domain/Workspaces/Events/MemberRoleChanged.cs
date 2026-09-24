using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Workspaces.Events;

public sealed record MemberRoleChanged(
    WorkspaceId WorkspaceId,
    UserId UserId,
    WorkspaceRole OldRole,
    WorkspaceRole NewRole
) : DomainEvent;