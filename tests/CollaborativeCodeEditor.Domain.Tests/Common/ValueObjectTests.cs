using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Tests.Common;

public sealed class ValueObjectTests
{
    private sealed record TestValue(string Value)
        : ValueObject;

    [Fact]
    public void ValueObjects_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var value1 = new TestValue("hello");
        var value2 = new TestValue("hello");

        // Act
        var result = value1 == value2;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValueObjects_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var value1 = new TestValue("hello");
        var value2 = new TestValue("world");

        // Act
        var result = value1 == value2;

        // Assert
        Assert.False(result);
    }
}