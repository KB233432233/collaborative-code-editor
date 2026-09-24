using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Executions.Events;

namespace CollaborativeCodeEditor.Domain.Executions;

public sealed class Execution : AggregateRoot<ExecutionId>
{
    public DocumentId DocumentId { get; }

    public UserId RequestedByUserId { get; }

    public ProgrammingLanguage Language { get; }

    public string SourceCode { get; }

    public ExecutionStatus Status { get; private set; }

    public ExecutionOutput Output { get; private set; }

    public DateTimeOffset RequestedAt { get; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    private Execution(
        ExecutionId id,
        DocumentId documentId,
        UserId requestedByUserId,
        ProgrammingLanguage language,
        string sourceCode,
        DateTimeOffset requestedAt)
        : base(id)
    {
        DocumentId = documentId;
        RequestedByUserId = requestedByUserId;
        Language = language;
        SourceCode = sourceCode;

        Status = ExecutionStatus.Requested;
        Output = ExecutionOutput.Empty;

        RequestedAt = requestedAt;
    }

    public static Execution Request(
    ExecutionId id,
    DocumentId documentId,
    UserId requestedByUserId,
    ProgrammingLanguage language,
    string sourceCode,
    DateTimeOffset requestedAt)
    {
        if (id.IsEmpty)
            throw new ArgumentException(
                "Execution ID cannot be empty.",
                nameof(id));

        if (documentId.IsEmpty)
            throw new ArgumentException(
                "Document ID cannot be empty.",
                nameof(documentId));

        if (requestedByUserId.IsEmpty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(requestedByUserId));

        if (sourceCode is null)
            throw new ArgumentNullException(
                nameof(sourceCode));

        var execution = new Execution(
            id,
            documentId,
            requestedByUserId,
            language,
            sourceCode,
            requestedAt);

        execution.AddDomainEvent(
            new ExecutionRequested(
                execution.Id,
                execution.DocumentId,
                execution.RequestedByUserId));

        return execution;
    }

    public void Queue()
    {
        if (Status != ExecutionStatus.Requested)
        {
            throw new InvalidOperationException(
                "Only requested executions can be queued.");
        }

        Status = ExecutionStatus.Queued;
    }

    public void Start(DateTimeOffset startedAt)
    {
        if (Status != ExecutionStatus.Queued)
        {
            throw new InvalidOperationException(
                "Only queued executions can be started.");
        }

        Status = ExecutionStatus.Running;
        StartedAt = startedAt;
    }

    public void Complete(
    ExecutionOutput output,
    DateTimeOffset completedAt)
    {
        if (Status != ExecutionStatus.Running)
        {
            throw new InvalidOperationException(
                "Only running executions can be completed.");
        }

        Output = output;
        Status = ExecutionStatus.Completed;
        CompletedAt = completedAt;

        AddDomainEvent(
            new ExecutionFinished(Id, Status));
    }

    public void Fail(
    ExecutionOutput output,
    DateTimeOffset completedAt)
    {
        if (Status != ExecutionStatus.Running)
        {
            throw new InvalidOperationException(
                "Only running executions can fail.");
        }

        Output = output;
        Status = ExecutionStatus.Failed;
        CompletedAt = completedAt;

        AddDomainEvent(
            new ExecutionFinished(Id, Status));
    }

    public void Timeout(
    DateTimeOffset completedAt)
    {
        if (Status != ExecutionStatus.Running)
        {
            throw new InvalidOperationException(
                "Only running executions can time out.");
        }

        Status = ExecutionStatus.TimedOut;
        CompletedAt = completedAt;

        AddDomainEvent(
            new ExecutionFinished(Id, Status));
    }
}