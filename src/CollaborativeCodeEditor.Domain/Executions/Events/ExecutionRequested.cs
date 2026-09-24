using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Domain.Executions.Events;

public sealed record ExecutionRequested(
    ExecutionId ExecutionId,
    DocumentId DocumentId,
    UserId RequestedByUserId
) : DomainEvent;