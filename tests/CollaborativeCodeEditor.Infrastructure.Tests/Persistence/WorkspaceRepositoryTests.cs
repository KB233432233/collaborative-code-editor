using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace CollaborativeCodeEditor.Infrastructure.Tests.Persistence;

public sealed class WorkspaceRepositoryTests
    : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public WorkspaceRepositoryTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistWorkspace()
    {
        var repository =
            new WorkspaceRepository(_database.DbContext);

        var workspaceId = WorkspaceId.New();
        var ownerId = new UserId(Guid.NewGuid());
        var workspaceName = WorkspaceName.Create("Test Workspace");

        var workspace = Workspace.Create(
            workspaceId,
            workspaceName,
            ownerId);

        await repository.AddAsync(workspace);

        await _database.DbContext.SaveChangesAsync();

        var result = await repository.GetByIdAsync(workspaceId);

        result.Should().NotBeNull();

        result!.Id.Should().Be(workspaceId);
        result.Name.Should().Be(workspaceName);
        result.OwnerId.Should().Be(ownerId);

        result.Members.Should().ContainSingle();

        var owner = result.Members.Single();

        owner.UserId.Should().Be(ownerId);
        owner.Role.Should().Be(WorkspaceRole.Owner);
        owner.JoinedAt.Should().NotBe(default);
    }
}