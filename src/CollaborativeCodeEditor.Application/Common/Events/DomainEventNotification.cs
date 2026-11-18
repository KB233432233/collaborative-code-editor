using CollaborativeCodeEditor.Domain.Common;
using MediatR;

namespace CollaborativeCodeEditor.Application.Common.Events;

public sealed record DomainEventNotification<TDomainEvent>(
    TDomainEvent DomainEvent) : INotification
    where TDomainEvent : DomainEvent;