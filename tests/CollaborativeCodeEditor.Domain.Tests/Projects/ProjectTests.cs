using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Projects.Events;
using CollaborativeCodeEditor.Domain.Workspaces;

namespace CollaborativeCodeEditor.Domain.Tests.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProject()
    {
        // Arrange
        var projectId = ProjectId.New();
        var workspaceId = WorkspaceId.New();
        var name = ProjectName.Create("Compiler Project");

        // Act
        var project = Project.Create(
            projectId,
            workspaceId,
            name);

        // Assert
        Assert.Equal(projectId, project.Id);
        Assert.Equal(workspaceId, project.WorkspaceId);
        Assert.Equal(name, project.Name);
    }

    [Fact]
    public void Create_WithEmptyWorkspaceId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            Project.Create(
                ProjectId.New(),
                WorkspaceId.Empty,
                ProjectName.Create("Project")));
    }

    [Fact]
    public void Rename_WithNewName_ShouldRenameProject()
    {
        // Arrange
        var project = Project.Create(
            ProjectId.New(),
            WorkspaceId.New(),
            ProjectName.Create("Old Name"));

        var newName = ProjectName.Create("New Name");

        // Act
        project.Rename(newName);

        // Assert
        Assert.Equal(newName, project.Name);
    }

    [Fact]
    public void Create_ShouldRaiseProjectCreatedEvent()
    {
        // Arrange
        var projectId = ProjectId.New();
        var workspaceId = WorkspaceId.New();

        // Act
        var project = Project.Create(
            projectId,
            workspaceId,
            ProjectName.Create("Project"));

        // Assert
        var domainEvent =
            Assert.Single(project.DomainEvents);

        var projectCreated =
            Assert.IsType<ProjectCreated>(domainEvent);

        Assert.Equal(
            projectId,
            projectCreated.ProjectId);

        Assert.Equal(
            workspaceId,
            projectCreated.WorkspaceId);
    }
}