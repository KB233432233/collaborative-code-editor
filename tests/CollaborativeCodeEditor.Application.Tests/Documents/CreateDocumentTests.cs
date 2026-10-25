using CollaborativeCodeEditor.Application.Documents.Commands.CreateDocument;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CollaborativeCodeEditor.Application.Tests.Documents;

public sealed class CreateDocumentTests
    : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public CreateDocumentTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task CreateDocument_Should_PersistDocument()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var projectId = await SeedProjectAsync(ownerId);
        var documentName = UniqueName("Document.cs");

        const string content = "Console.WriteLine(\"Hello\");";

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateDocumentCommand(
                projectId.Value,
                documentName,
                (int)ProgrammingLanguage.CSharp,
                content));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var document = await FindDocumentAsync(result.Value);

        document.Should().NotBeNull();

        document!.Name.Value
            .Should()
            .Be(documentName);

        document.ProjectId
            .Should()
            .Be(projectId);

        document.Language
            .Should()
            .Be(ProgrammingLanguage.CSharp);

        document.Content
            .Should()
            .Be(content);

        // A newly created document starts at version 0.
        document.Version
            .Should()
            .Be(DocumentVersion.Initial);
    }

    [Fact]
    public async Task CreateDocument_WithNullContent_Should_PersistEmptyContent()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var projectId = await SeedProjectAsync(ownerId);

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateDocumentCommand(
                projectId.Value,
                UniqueName("Empty.py"),
                (int)ProgrammingLanguage.Python,
                null));

        // Assert
        result.IsSuccess.Should().BeTrue();

        var document = await FindDocumentAsync(result.Value);

        document.Should().NotBeNull();

        document!.Content
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task CreateDocument_WhenCallerIsNotAMember_Should_Fail()
    {
        // Arrange: the project belongs to someone else's workspace.
        var ownerId = new UserId(Guid.NewGuid());
        var outsiderId = new UserId(Guid.NewGuid());
        var projectId = await SeedProjectAsync(ownerId);

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(outsiderId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateDocumentCommand(
                projectId.Value,
                UniqueName("Document.cs"),
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().NotBeNull();

        result.Error!.Code
            .Should()
            .Be("Workspace.AccessDenied");

        (await DocumentCountAsync(projectId))
            .Should()
            .Be(0);
    }

    [Fact]
    public async Task CreateDocument_WhenProjectDoesNotExist_Should_Fail()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var unknownProjectId = Guid.NewGuid();

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(userId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new CreateDocumentCommand(
                unknownProjectId,
                UniqueName("Document.cs"),
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().NotBeNull();

        result.Error!.Code
            .Should()
            .Be("Project.NotFound");
    }

    [Fact]
    public async Task CreateDocument_WithDuplicateName_Should_Fail()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var projectId = await SeedProjectAsync(ownerId);
        var documentName = UniqueName("Document.cs");

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var firstResult = await mediator.Send(
            new CreateDocumentCommand(
                projectId.Value,
                documentName,
                (int)ProgrammingLanguage.CSharp,
                null));

        firstResult.IsSuccess.Should().BeTrue();

        // Act
        var secondResult = await mediator.Send(
            new CreateDocumentCommand(
                projectId.Value,
                documentName,
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        secondResult.IsSuccess.Should().BeFalse();

        secondResult.Error.Should().NotBeNull();

        secondResult.Error!.Code
            .Should()
            .Be("Document.NameAlreadyExists");

        (await DocumentCountAsync(projectId))
            .Should()
            .Be(1);
    }

    [Fact]
    public async Task CreateDocument_WithSameNameInDifferentProjects_Should_Succeed()
    {
        // The unique index is scoped to (ProjectId, Name), so the same
        // filename must be allowed across projects.
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var firstProjectId = await SeedProjectAsync(ownerId);
        var secondProjectId = await SeedProjectAsync(ownerId);
        var documentName = UniqueName("Program.cs");

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator =
            serviceProvider.GetRequiredService<ISender>();

        var firstResult = await mediator.Send(
            new CreateDocumentCommand(
                firstProjectId.Value,
                documentName,
                (int)ProgrammingLanguage.CSharp,
                null));

        // Act
        var secondResult = await mediator.Send(
            new CreateDocumentCommand(
                secondProjectId.Value,
                documentName,
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();

        secondResult.Value
            .Should()
            .NotBe(firstResult.Value);
    }

    [Fact]
    public async Task CreateDocument_WithEmptyName_Should_Fail_Validation()
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
            new CreateDocumentCommand(
                Guid.NewGuid(),
                string.Empty,
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Document name is required*");
    }

    [Fact]
    public async Task CreateDocument_WithNameTooLong_Should_Fail_Validation()
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
            DocumentName.MaxLength + 1);

        // Act
        var result = () => mediator.Send(
            new CreateDocumentCommand(
                Guid.NewGuid(),
                tooLong,
                (int)ProgrammingLanguage.CSharp,
                null));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*cannot exceed 255 characters*");
    }

    [Fact]
    public async Task CreateDocument_WithUnsupportedLanguage_Should_Fail_Validation()
    {
        // Without IsInEnum() this would persist an undefined enum value
        // and only surface later, when rendering the document.
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
            new CreateDocumentCommand(
                Guid.NewGuid(),
                UniqueName("Document.xyz"),
                999,
                null));

        // Assert
        await result.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage("*Language is not supported*");
    }

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

    // Seeded directly rather than through CreateProjectCommand, so a
    // regression in project creation cannot produce confusing failures here.
    private async Task<ProjectId> SeedProjectAsync(UserId ownerId)
    {
        var workspaceId = await SeedWorkspaceAsync(ownerId);

        var projectId = ProjectId.New();

        var project = Project.Create(
            projectId,
            workspaceId,
            ProjectName.Create(UniqueName("Project")));

        _database.DbContext.Projects.Add(project);

        await _database.DbContext.SaveChangesAsync();

        return projectId;
    }

    private async Task<Document?> FindDocumentAsync(Guid documentId)
    {
        await using var context = CreateContext();

        return await context.Documents
            .SingleOrDefaultAsync(
                document => document.Id == new DocumentId(documentId));
    }

    private async Task<int> DocumentCountAsync(ProjectId projectId)
    {
        await using var context = CreateContext();

        return await context.Documents
            .CountAsync(
                document => document.ProjectId == projectId);
    }

    private AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_database.ConnectionString)
                .Options);

    private static string UniqueName(string prefix)
        => $"{prefix}-{Guid.NewGuid():N}";
}