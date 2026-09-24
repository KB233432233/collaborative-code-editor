namespace CollaborativeCodeEditor.Domain.Collaboration;

public readonly record struct OperationIdentifier(
    ReplicaId ReplicaId,
    long SequenceNumber);