namespace CollaborativeCodeEditor.Application.Common.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken = default);
}