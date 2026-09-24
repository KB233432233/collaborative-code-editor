using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Tests.Common;

public sealed class AggregateRootTests
{
    private sealed record TestEvent
        : DomainEvent;

    private sealed class TestAggregate(Guid id)
        : AggregateRoot<Guid>(id)
    {
        public void RaiseEvent()
        {
            AddDomainEvent(new TestEvent());
        }
    }

    [Fact]
    public void Aggregate_ShouldStoreDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());

        // Act
        aggregate.RaiseEvent();

        // Assert
        Assert.Single(aggregate.DomainEvents);
    }

    [Fact]
    public void Aggregate_ShouldClearDomainEvents()
    {
        // Arrange
        var aggregate = new TestAggregate(Guid.NewGuid());

        aggregate.RaiseEvent();

        // Act
        aggregate.ClearDomainEvents();

        // Assert
        Assert.Empty(aggregate.DomainEvents);
    }
}