namespace CollaborativeCodeEditor.Domain.Collaboration;

public readonly record struct CrdtElementId(
    ReplicaId ReplicaId,
    long SequenceNumber);