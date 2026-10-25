using CollaborativeCodeEditor.Application.Projects.Commands.CreateProject;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CollaborativeCodeEditor.Application.Tests.Projects;

public sealed class CreateProjectTests
    : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public CreateProjectTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task CreateProject_Should_PersistProject()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var workspaceId = await SeedWorkspaceAsync(ownerId);
        var projectName = UniqueName("Project");

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateProjectCommand(
                workspaceId.Value,
                projectName));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var project = await FindProjectAsync(result.Value);

        project.Should().NotBeNull();

        project!.Name.Value
            .Should()
            .Be(projectName);

        project.WorkspaceId
            .Should()
            .Be(workspaceId);
    }

    [Fact]
    public async Task CreateProject_WhenCallerIsNotAMember_Should_Fail()
    {
        // Arrange: the workspace belongs to someone else.
        var ownerId = new UserId(Guid.NewGuid());
        var outsiderId = new UserId(Guid.NewGuid());
        var workspaceId = await SeedWorkspaceAsync(ownerId);

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(outsiderId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateProjectCommand(
                workspaceId.Value,
                UniqueName("Project")));

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().NotBeNull();

        result.Error!.Code
            .Should()
            .Be("Workspace.AccessDenied");

        await ProjectCountAsync(workspaceId)
            .ContinueWith(count => count.Result.Should().Be(0));
    }

    [Fact]
    public async Task CreateProject_WhenWorkspaceDoesNotExist_Should_Fail()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var unknownWorkspaceId = Guid.NewGuid();

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(userId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateProjectCommand(
                unknownWorkspaceId,
                UniqueName("Project")));

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().NotBeNull();

        result.Error!.Code
            .Should()
            .Be("Workspace.NotFound");
    }

    [Fact]
    public async Task CreateProject_WithDuplicateName_Should_Fail()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var workspaceId = await SeedWorkspaceAsync(ownerId);
        var projectName = UniqueName("Project");

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var firstResult = await mediator.Send(
            new CreateProjectCommand(
                workspaceId.Value,
                projectName));

        firstResult.IsSuccess.Should().BeTrue();

        // Act
        var secondResult = await mediator.Send(
            new CreateProjectCommand(
                workspaceId.Value,
                projectName));

        // Assert
        secondResult.IsSuccess.Should().BeFalse();

        secondResult.Error.Should().NotBeNull();

        secondResult.Error!.Code
            .Should()
            .Be("Project.NameAlreadyExists");

        (await ProjectCountAsync(workspaceId))
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task CreateProject_WithEmptyName_Should_Fail_Validation()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(userId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = () => mediator.Send(
            new CreateProjectCommand(
                Guid.NewGuid(),
                string.Empty));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Project name is required*");
    }

    [Fact]
    public async Task CreateProject_WithNameTooLong_Should_Fail_Validation()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(userId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var tooLong = new string(
            'a',
            ProjectName.MaxLength + 1);

        // Act
        var result = () => mediator.Send(
            new CreateProjectCommand(
                Guid.NewGuid(),
                tooLong));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*cannot exceed 100 characters*");
    }

    // The workspace is seeded directly through the fixture's context rather
    // than via CreateWorkspaceCommand, so these tests make no assumption about
    // the caller already being registered as a user, and stay independent of
    // the workspace feature's own behaviour.
    private async Task<WorkspaceId> SeedWorkspaceAsync(UserId ownerId)
    {
        var workspaceId = WorkspaceId.New();

        var workspace = Workspace.Create(
            workspaceId,
            WorkspaceName.Create(UniqueName("Workspace")),
            ownerId);

        _database.DbContext.Workspaces.Add(workspace);

        await _database.DbContext.SaveChangesAsync();

        return workspaceId;
    }

    private async Task<Project?> FindProjectAsync(Guid projectId)
    {
        await using var context = CreateContext();

        return await context.Projects
            .SingleOrDefaultAsync(
                project => project.Id == new ProjectId(projectId));
    }

    private async Task<int> ProjectCountAsync(WorkspaceId workspaceId)
    {
        await using var context = CreateContext();

        return await context.Projects
            .CountAsync(
                project => project.WorkspaceId == workspaceId);
    }

    private AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_database.ConnectionString)
                .Options);

    private static string UniqueName(string prefix)
        => $"{prefix}-{Guid.NewGuid():N}";
}