using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Domain.Workspaces.Events;

namespace CollaborativeCodeEditor.Domain.Tests.Workspaces;

public sealed class WorkspaceTests
{
    [Fact]
    public void Create_ShouldCreateWorkspace()
    {
        // Arrange
        var workspaceId = WorkspaceId.New();
        var ownerId = UserId.New();
        var name = WorkspaceName.Create("My Workspace");

        // Act
        var workspace = Workspace.Create(
            workspaceId,
            name,
            ownerId);

        // Assert
        Assert.Equal(workspaceId, workspace.Id);
        Assert.Equal(name, workspace.Name);
        Assert.Equal(ownerId, workspace.OwnerId);
    }
    [Fact]
    public void Create_ShouldAddOwnerAsOwnerMember()
    {
        // Arrange
        var ownerId = UserId.New();

        // Act
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        // Assert
        var owner = Assert.Single(
            workspace.Members);

        Assert.Equal(
            ownerId,
            owner.UserId);

        Assert.Equal(
            WorkspaceRole.Owner,
            owner.Role);
    }
    [Fact]
    public void AddMember_ShouldAddMember()
    {
        // Arrange
        var ownerId = UserId.New();
        var memberId = UserId.New();

        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        // Act
        workspace.AddMember(
            memberId,
            WorkspaceRole.Editor);

        // Assert
        var member = workspace.Members
            .Single(x => x.UserId == memberId);

        Assert.Equal(
            WorkspaceRole.Editor,
            member.Role);
    }
    [Fact]
    public void AddMember_WhenUserAlreadyMember_ShouldThrow()
    {
        // Arrange
        var ownerId = UserId.New();
        var memberId = UserId.New();

        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        workspace.AddMember(
            memberId,
            WorkspaceRole.Editor);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            workspace.AddMember(
                memberId,
                WorkspaceRole.Viewer));
    }
    [Fact]
    public void RemoveMember_WhenUserIsOwner_ShouldThrow()
    {
        // Arrange
        var ownerId = UserId.New();

        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            workspace.RemoveMember(ownerId));
    }
    [Fact]
    public void ChangeMemberRole_ShouldChangeRole()
    {
        // Arrange
        var ownerId = UserId.New();
        var memberId = UserId.New();

        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        workspace.AddMember(
            memberId,
            WorkspaceRole.Viewer);

        // Act
        workspace.ChangeMemberRole(
            memberId,
            WorkspaceRole.Editor);

        // Assert
        var member = workspace.Members
            .Single(x => x.UserId == memberId);

        Assert.Equal(
            WorkspaceRole.Editor,
            member.Role);
    }
    [Fact]
    public void TransferOwnership_ShouldChangeOwner()
    {
        // Arrange
        var ownerId = UserId.New();
        var newOwnerId = UserId.New();

        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        workspace.AddMember(
            newOwnerId,
            WorkspaceRole.Editor);

        // Act
        workspace.TransferOwnership(newOwnerId);

        // Assert
        Assert.Equal(
            newOwnerId,
            workspace.OwnerId);

        var previousOwner = workspace.Members
            .Single(x => x.UserId == ownerId);

        var newOwner = workspace.Members
            .Single(x => x.UserId == newOwnerId);

        Assert.Equal(
            WorkspaceRole.Editor,
            previousOwner.Role);

        Assert.Equal(
            WorkspaceRole.Owner,
            newOwner.Role);
    }
    [Fact]
    public void Create_ShouldRaiseWorkspaceCreatedEvent()
    {
        // Arrange
        var ownerId = UserId.New();

        // Act
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            ownerId);

        // Assert
        var domainEvent = Assert.Single(
            workspace.DomainEvents);

        var workspaceCreated =
            Assert.IsType<WorkspaceCreated>(domainEvent);

        Assert.Equal(
            workspace.Id,
            workspaceCreated.WorkspaceId);

        Assert.Equal(
            ownerId,
            workspaceCreated.OwnerId);
    }
    [Fact]
    public void AddMember_ShouldRaiseMemberAddedEvent()
    {
        // Arrange
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            UserId.New());

        var memberId = UserId.New();

        // Act
        workspace.AddMember(
            memberId,
            WorkspaceRole.Editor);

        // Assert
        var domainEvent = workspace.DomainEvents
            .OfType<MemberAddedToWorkspace>()
            .Single();

        Assert.Equal(
            memberId,
            domainEvent.UserId);

        Assert.Equal(
            WorkspaceRole.Editor,
            domainEvent.Role);
    }
    [Fact]
    public void RemoveMember_ShouldRaiseMemberRemovedEvent()
    {
        // Arrange 
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            UserId.New()
        );
        var memberId = UserId.New();

        // Act
        workspace.AddMember(
            memberId,
            WorkspaceRole.Editor);

        workspace.RemoveMember(memberId);

        var domainEvent = workspace.DomainEvents
            .OfType<MemberRemovedFromWorkspace>()
            .Single();

        Assert.Equal(
            memberId,
            domainEvent.UserId);
    }

    [Fact]
    public void ChangeMemberRole_ShouldRaiseMemberRoleChangedEvent()
    {
        // Arrange
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create("Workspace"),
            UserId.New());

        var memberId = UserId.New();

        workspace.AddMember(
            memberId,
            WorkspaceRole.Viewer);

        // Act
        workspace.ChangeMemberRole(
            memberId,
            WorkspaceRole.Editor);

        // Assert
        var domainEvent = workspace.DomainEvents
            .OfType<MemberRoleChanged>()
            .Single();

        Assert.Equal(
            memberId,
            domainEvent.UserId);

        Assert.Equal(
            WorkspaceRole.Editor,
            domainEvent.NewRole);
    }
}