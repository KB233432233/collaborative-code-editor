using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Tests.Common;

public sealed class DomainEventTests
{
    private sealed record TestDomainEvent
        : DomainEvent;

    [Fact]
    public void DomainEvent_ShouldHaveOccurredAt()
    {
        // Act
        var domainEvent = new TestDomainEvent();

        // Assert
        Assert.NotEqual(default, domainEvent.OccurredAt);
    }
}