using CollaborativeCodeEditor.Application.Common.Events;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using Xunit;

namespace CollaborativeCodeEditor.Infrastructure.Tests.Persistence;

public sealed class EfUnitOfWorkTests : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public EfUnitOfWorkTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutExplicitTransaction_DispatchesDomainEvents()
    {
        // Arrange
        var dispatcher = new RecordingDomainEventDispatcher();

        var unitOfWork = new EfUnitOfWork(
            _database.DbContext,
            dispatcher);

        var workspace = CreateWorkspaceWithDomainEvent();

        _database.DbContext.Workspaces.Add(workspace);

        // Act
        await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.NotEmpty(dispatcher.DispatchedEvents);
        Assert.Empty(workspace.DomainEvents);
    }

    [Fact]
    public async Task SaveChangesAsync_InsideTransaction_DoesNotDispatchImmediately()
    {
        // Arrange
        var dispatcher = new RecordingDomainEventDispatcher();

        var unitOfWork = new EfUnitOfWork(
            _database.DbContext,
            dispatcher);

        var workspace = CreateWorkspaceWithDomainEvent();

        // Act
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _database.DbContext.Workspaces.Add(workspace);

            await unitOfWork.SaveChangesAsync();

            // Assert while transaction is still active
            Assert.Empty(dispatcher.DispatchedEvents);
            Assert.NotEmpty(workspace.DomainEvents);

            return true;
        });

        // Assert after transaction commits
        Assert.NotEmpty(dispatcher.DispatchedEvents);
        Assert.Empty(workspace.DomainEvents);
    }

    [Fact]
    public async Task ExecuteInTransactionAsync_WhenActionFails_DoesNotDispatchDomainEvents()
    {
        // Arrange
        var dispatcher = new RecordingDomainEventDispatcher();

        var unitOfWork = new EfUnitOfWork(
            _database.DbContext,
            dispatcher);

        var workspace = CreateWorkspaceWithDomainEvent();

        // Act
        async Task<bool> FailingTransaction()
        {
            _database.DbContext.Workspaces.Add(workspace);

            await unitOfWork.SaveChangesAsync();

            throw new InvalidOperationException("Simulated failure.");
        }

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => unitOfWork.ExecuteInTransactionAsync<bool>(
                FailingTransaction));

        // Assert
        Assert.Empty(dispatcher.DispatchedEvents);
        Assert.NotEmpty(workspace.DomainEvents);
    }

    private static Workspace CreateWorkspaceWithDomainEvent()
    {
        // Use the same Workspace factory and arguments
        // used by your existing Workspace domain tests.
        throw new NotImplementedException();
    }
}