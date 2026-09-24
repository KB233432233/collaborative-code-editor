using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Workspaces.Events;

public sealed record MemberRemovedFromWorkspace(
    WorkspaceId WorkspaceId,
    UserId UserId
) : DomainEvent;