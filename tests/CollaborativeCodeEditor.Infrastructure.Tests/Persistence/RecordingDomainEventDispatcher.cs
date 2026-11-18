using CollaborativeCodeEditor.Application.Common.Events;
using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Infrastructure.Tests.Persistence;

internal sealed class RecordingDomainEventDispatcher
    : IDomainEventDispatcher
{
    public List<DomainEvent> DispatchedEvents { get; } = [];

    public bool ShouldThrow { get; set; }

    public Task DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        if (ShouldThrow)
        {
            throw new InvalidOperationException(
                "Simulated domain event dispatch failure.");
        }

        DispatchedEvents.AddRange(domainEvents);

        return Task.CompletedTask;
    }
}