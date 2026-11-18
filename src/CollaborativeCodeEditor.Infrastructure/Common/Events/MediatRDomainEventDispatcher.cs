using CollaborativeCodeEditor.Application.Common.Events;
using CollaborativeCodeEditor.Domain.Common;
using MediatR;

namespace CollaborativeCodeEditor.Infrastructure.Common.Events;

public sealed class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(
        IEnumerable<DomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(
                notificationType,
                domainEvent) as INotification;

            if (notification is null)
            {
                throw new InvalidOperationException(
                    $"Could not create a notification for {domainEvent.GetType().Name}.");
            }

            await _publisher.Publish(notification, cancellationToken);
        }
    }
}