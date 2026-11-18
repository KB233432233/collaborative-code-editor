
using CollaborativeCodeEditor.Application.Common.Events;
using CollaborativeCodeEditor.Application.Common.Exceptions;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    private bool _hasSavedChangesInCurrentTransaction;

    public EfUnitOfWork(
        AppDbContext dbContext,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _dbContext = dbContext;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException(
                "A concurrency conflict occurred while saving changes.",
                exception);
        }

        if (_dbContext.Database.CurrentTransaction is not null)
        {
            _hasSavedChangesInCurrentTransaction = true;
            return;
        }

        await DispatchDomainEventsAsync(cancellationToken);
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        _hasSavedChangesInCurrentTransaction = false;

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        T result;

        try
        {
            result = await action();

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            _hasSavedChangesInCurrentTransaction = false;
            throw;
        }

        var hasSavedChanges = _hasSavedChangesInCurrentTransaction;
        _hasSavedChangesInCurrentTransaction = false;

        if (hasSavedChanges)
        {
            await DispatchDomainEventsAsync(cancellationToken);
        }

        return result;
    }

    private async Task DispatchDomainEventsAsync(
        CancellationToken cancellationToken)
    {
        var aggregates = _dbContext.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Select(entry => entry.Entity)
            .Where(aggregate => aggregate.DomainEvents.Count > 0)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();

        if (domainEvents.Count == 0)
        {
            return;
        }

        await _domainEventDispatcher.DispatchAsync(
            domainEvents,
            cancellationToken);

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }
    }
}
