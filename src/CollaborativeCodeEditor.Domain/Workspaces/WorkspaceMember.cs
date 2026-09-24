using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Workspaces;

public sealed class WorkspaceMember
{
    public WorkspaceId WorkspaceId { get; private set; }

    public UserId UserId { get; }

    public WorkspaceRole Role { get; private set; }

    public DateTimeOffset JoinedAt { get; }

    internal WorkspaceMember(
        WorkspaceId workspaceId,
        UserId userId,
        WorkspaceRole role,
        DateTimeOffset joinedAt)
    {
        if (workspaceId.IsEmpty)
        {
            throw new ArgumentException(
                "Workspace ID cannot be empty.",
                nameof(workspaceId));
        }

        if (userId.IsEmpty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        WorkspaceId = workspaceId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
    }

    internal void ChangeRole(WorkspaceRole role)
    {
        Role = role;
    }
}