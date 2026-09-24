using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces.Events;

namespace CollaborativeCodeEditor.Domain.Workspaces;

public sealed class Workspace : AggregateRoot<WorkspaceId>
{
    private readonly List<WorkspaceMember> _members = [];

    public WorkspaceName Name { get; private set; }

    public UserId OwnerId { get; private set; }

    public IReadOnlyCollection<WorkspaceMember> Members =>
        _members.AsReadOnly();

    private Workspace(
        WorkspaceId id,
        WorkspaceName name,
        UserId ownerId)
        : base(id)
    {
        Name = name;
        OwnerId = ownerId;
    }

    public static Workspace Create(
        WorkspaceId id,
        WorkspaceName name,
        UserId ownerId)
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Workspace ID cannot be empty.",
                nameof(id));
        }

        if (ownerId.IsEmpty)
        {
            throw new ArgumentException(
                "Owner ID cannot be empty.",
                nameof(ownerId));
        }

        var workspace = new Workspace(
            id,
            name,
            ownerId);

        workspace._members.Add(
            new WorkspaceMember(
            id,
            ownerId,
            WorkspaceRole.Owner,
            DateTimeOffset.UtcNow));

        workspace.AddDomainEvent(
            new WorkspaceCreated(
                workspace.Id,
                workspace.OwnerId));

        return workspace;
    }
    public void AddMember(
    UserId userId,
    WorkspaceRole role)
    {
        if (userId.IsEmpty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        if (role == WorkspaceRole.Owner)
        {
            throw new InvalidOperationException(
                "A workspace can only have one owner.");
        }

        if (_members.Any(x => x.UserId == userId))
        {
            throw new InvalidOperationException(
                "User is already a member of this workspace.");
        }

        var member = new WorkspaceMember(
            Id,
            userId,
            role,
            DateTimeOffset.UtcNow);

        _members.Add(member);

        AddDomainEvent(
            new MemberAddedToWorkspace(
                Id,
                userId,
                role));
    }
    public void RemoveMember(UserId userId)
    {
        if (userId.IsEmpty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        if (userId == OwnerId)
        {
            throw new InvalidOperationException(
                "The workspace owner cannot be removed.");
        }

        var member = _members
            .SingleOrDefault(x => x.UserId == userId);

        if (member is null)
        {
            throw new InvalidOperationException(
                "User is not a member of this workspace.");
        }

        _members.Remove(member);

        AddDomainEvent(
            new MemberRemovedFromWorkspace(
                Id,
                userId));
    }
    public void ChangeMemberRole(
    UserId userId,
    WorkspaceRole newRole)
    {
        if (userId.IsEmpty)
        {
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));
        }

        if (newRole == WorkspaceRole.Owner)
        {
            throw new InvalidOperationException(
                "Use ownership transfer to change the workspace owner.");
        }

        var member = _members
            .SingleOrDefault(x => x.UserId == userId);

        if (member is null)
        {
            throw new InvalidOperationException(
                "User is not a member of this workspace.");
        }

        if (member.Role == newRole)
            return;

        var oldRole = member.Role;

        member.ChangeRole(newRole);

        AddDomainEvent(
            new MemberRoleChanged(
                Id,
                userId,
                oldRole,
                newRole));
    }
    public void TransferOwnership(UserId newOwnerId)
    {
        if (newOwnerId.IsEmpty)
        {
            throw new ArgumentException(
                "New owner ID cannot be empty.",
                nameof(newOwnerId));
        }

        if (newOwnerId == OwnerId)
            return;

        var newOwner = _members
            .SingleOrDefault(x => x.UserId == newOwnerId);

        if (newOwner is null)
        {
            throw new InvalidOperationException(
                "New owner must already be a workspace member.");
        }

        var previousOwnerId = OwnerId;

        var previousOwner = _members
            .Single(x => x.UserId == OwnerId);

        previousOwner.ChangeRole(
            WorkspaceRole.Editor);

        newOwner.ChangeRole(
            WorkspaceRole.Owner);

        OwnerId = newOwnerId;

        // We'll introduce an ownership-transfer event
        // once we decide whether consumers need it.
    }
    public void Rename(WorkspaceName name)
    {
        if (Name == name)
            return;

        Name = name;
    }
}