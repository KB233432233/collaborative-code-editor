using CollaborativeCodeEditor.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using CollaborativeCodeEditor.Application.Common.Exceptions;

namespace CollaborativeCodeEditor.Infrastructure.Persistence;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public EfUnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var result = await action();

            await transaction.CommitAsync(
                cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
        }
    }
}