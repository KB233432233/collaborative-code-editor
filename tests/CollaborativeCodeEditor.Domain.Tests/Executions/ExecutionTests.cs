using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Executions;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Tests.Executions;

public sealed class ExecutionTests
{
    [Fact]
    public void Request_ShouldCreateExecution()
    {
        // Act
        var execution = Execution.Request(
            ExecutionId.New(),
            DocumentId.New(),
            UserId.New(),
            ProgrammingLanguage.CSharp,
            "Console.WriteLine(\"Hello\");",
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(
            ExecutionStatus.Requested,
            execution.Status);
    }

    [Fact]
    public void Execution_ShouldFollowValidLifecycle()
    {
        // Arrange
        var execution = Execution.Request(
            ExecutionId.New(),
            DocumentId.New(),
            UserId.New(),
            ProgrammingLanguage.CSharp,
            "Console.WriteLine(\"Hello\");",
            DateTimeOffset.UtcNow);

        // Act
        execution.Queue();

        execution.Start(DateTimeOffset.UtcNow);

        execution.Complete(
            ExecutionOutput.Create(
                "Hello",
                null),
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(
            ExecutionStatus.Completed,
            execution.Status);

        Assert.Equal(
            "Hello",
            execution.Output.StandardOutput);
    }

    [Fact]
    public void Complete_WhenNotRunning_ShouldThrow()
    {
        var execution = Execution.Request(
            ExecutionId.New(),
            DocumentId.New(),
            UserId.New(),
            ProgrammingLanguage.CSharp,
            "",
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(
            () => execution.Complete(
                ExecutionOutput.Empty,
                DateTimeOffset.UtcNow));
    }
}