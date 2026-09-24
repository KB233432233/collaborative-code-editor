using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Executions.Events;

public sealed record ExecutionFinished(
    ExecutionId ExecutionId,
    ExecutionStatus Status
) : DomainEvent;