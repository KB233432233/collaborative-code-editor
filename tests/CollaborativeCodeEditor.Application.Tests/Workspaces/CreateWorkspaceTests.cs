using CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CollaborativeCodeEditor.Application.Tests.Workspaces;

public sealed class CreateWorkspaceTests
    : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public CreateWorkspaceTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task CreateWorkspace_ShouldPersistWorkspace()
    {
        var userId = new UserId(Guid.NewGuid());

        var currentUser =
            new FakeCurrentUser(userId);

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                currentUser);

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var command =
            new CreateWorkspaceCommand(
                "My Workspace");

        var result =
            await mediator.Send(command);

        result.IsSuccess.Should().BeTrue();

        var workspaceId = result.Value;

        await using var verificationContext =
            new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseNpgsql(_database.ConnectionString)
                    .Options);

        var workspace =
            await verificationContext.Workspaces
                .Include(x => x.Members)
                .SingleOrDefaultAsync(
                    x => x.Id == new WorkspaceId(workspaceId));

        workspace.Should().NotBeNull();

        workspace!.Name.Value
            .Should()
            .Be("My Workspace");

        workspace.OwnerId
            .Should()
            .Be(userId);

        workspace.Members
            .Should()
            .ContainSingle();

        var owner =
            workspace.Members.Single();

        owner.UserId
            .Should()
            .Be(userId);

        owner.Role
            .Should()
            .Be(
                CollaborativeCodeEditor.Domain.Workspaces
                    .WorkspaceRole.Owner);
    }
}