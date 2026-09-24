using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Tests.Common;

public sealed class EntityTests
{
    private sealed class TestEntity(Guid id)
        : Entity<Guid>(id);

    [Fact]
    public void Entities_WithSameId_ShouldBeEqual()
    {
        // Arrange
        var id = Guid.NewGuid();

        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var result = entity1 == entity2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Entities_WithDifferentIds_ShouldNotBeEqual()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid());
        var entity2 = new TestEntity(Guid.NewGuid());

        // Act
        var result = entity1 == entity2;

        // Assert
        Assert.False(result);
    }
}