
using CollaborativeCodeEditor.Application.Common.Exceptions;
using CollaborativeCodeEditor.Application.Documents.Commands.UpdateDocument;
using CollaborativeCodeEditor.Application.Tests.Common;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CollaborativeCodeEditor.Application.Common.Events;
using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Application.Tests.Documents;

public sealed class UpdateDocumentTests : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public UpdateDocumentTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task UpdateDocument_WithCorrectVersion_ShouldPersistContentAndIncrementVersion()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var (projectId, documentId) = await SeedDocumentAsync(ownerId);

        const string newContent = "Console.WriteLine(\"Updated\");";

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator = serviceProvider.GetRequiredService<ISender>();

        // Act
        var result = await mediator.Send(
            new UpdateDocumentCommand(
                projectId.Value,
                documentId.Value,
                newContent,
                ExpectedVersion: 0));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(1);

        var document = await FindDocumentAsync(documentId);

        document.Should().NotBeNull();
        document!.Content.Should().Be(newContent);
        document.Version.Should().Be(new DocumentVersion(1));
    }

    [Fact]
    public async Task UpdateDocument_WithStaleVersion_ShouldReturnConcurrencyConflict()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var (projectId, documentId) = await SeedDocumentAsync(ownerId);

        const string firstUpdate = "First update";
        const string staleUpdate = "Stale update";

        await using var serviceProvider =
            TestServiceProvider.Create(
                _database.ConnectionString,
                new FakeCurrentUser(ownerId));

        var mediator = serviceProvider.GetRequiredService<ISender>();

        var firstResult = await mediator.Send(
            new UpdateDocumentCommand(
                projectId.Value,
                documentId.Value,
                firstUpdate,
                ExpectedVersion: 0));

        firstResult.IsSuccess.Should().BeTrue();

        // Act: the client still expects version 0, but the document is now at 1.
        var staleResult = await mediator.Send(
            new UpdateDocumentCommand(
                projectId.Value,
                documentId.Value,
                staleUpdate,
                ExpectedVersion: 0));

        // Assert
        staleResult.IsSuccess.Should().BeFalse();

        staleResult.Error.Should().NotBeNull();
        staleResult.Error!.Code.Should().Be("Document.ConcurrencyConflict");

        var document = await FindDocumentAsync(documentId);

        document.Should().NotBeNull();
        document!.Content.Should().Be(firstUpdate);
        document.Version.Should().Be(new DocumentVersion(1));
    }

    [Fact]
    public async Task UpdateDocument_WhenTwoContextsUpdateSameVersion_ShouldRejectSecondSave()
    {
        // Arrange
        var ownerId = new UserId(Guid.NewGuid());
        var (_, documentId) = await SeedDocumentAsync(ownerId);

        await using var firstContext = CreateContext();
        await using var secondContext = CreateContext();

        var firstDocument = await firstContext.Documents
            .SingleAsync(x => x.Id == documentId);

        var secondDocument = await secondContext.Documents
            .SingleAsync(x => x.Id == documentId);

        firstDocument.UpdateContent(
            "First concurrent update",
            DocumentVersion.Initial);

        secondDocument.UpdateContent(
            "Second concurrent update",
            DocumentVersion.Initial);

        var firstDispatcher = new NoOpDomainEventDispatcher();
        var secondDispatcher = new NoOpDomainEventDispatcher();

        var firstUnitOfWork = new EfUnitOfWork(
            firstContext,
            firstDispatcher);

        var secondUnitOfWork = new EfUnitOfWork(
            secondContext,
            secondDispatcher);

        // Act
        await firstUnitOfWork.SaveChangesAsync();

        var secondSave = async () =>
            await secondUnitOfWork.SaveChangesAsync();

        // Assert
        await secondSave.Should()
            .ThrowAsync<ConcurrencyConflictException>();

        var persistedDocument = await FindDocumentAsync(documentId);

        persistedDocument.Should().NotBeNull();
        persistedDocument!.Content.Should().Be("First concurrent update");
        persistedDocument.Version.Should().Be(new DocumentVersion(1));
    }

    private async Task<(ProjectId ProjectId, DocumentId DocumentId)>
        SeedDocumentAsync(UserId ownerId)
    {
        var workspaceId = WorkspaceId.New();

        var workspace = Workspace.Create(
            workspaceId,
            WorkspaceName.Create(UniqueName("Workspace")),
            ownerId,
            DateTimeOffset.UtcNow);

        var projectId = ProjectId.New();

        var project = Project.Create(
            projectId,
            workspaceId,
            ProjectName.Create(UniqueName("Project")));

        var documentId = DocumentId.New();

        var document = Document.Create(
            documentId,
            projectId,
            DocumentName.Create(UniqueName("Document.cs")),
            ProgrammingLanguage.CSharp,
            "Initial content");

        _database.DbContext.Workspaces.Add(workspace);
        _database.DbContext.Projects.Add(project);
        _database.DbContext.Documents.Add(document);

        await _database.DbContext.SaveChangesAsync();

        return (projectId, documentId);
    }

    private async Task<Document?> FindDocumentAsync(DocumentId documentId)
    {
        await using var context = CreateContext();

        return await context.Documents
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == documentId);
    }

    private AppDbContext CreateContext()
        => new(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_database.ConnectionString)
                .Options);

    private static string UniqueName(string prefix)
        => $"{prefix}-{Guid.NewGuid():N}";
    private sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task DispatchAsync(
            IEnumerable<DomainEvent> domainEvents,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}


