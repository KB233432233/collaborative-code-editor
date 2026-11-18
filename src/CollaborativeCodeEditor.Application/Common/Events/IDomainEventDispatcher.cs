using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}