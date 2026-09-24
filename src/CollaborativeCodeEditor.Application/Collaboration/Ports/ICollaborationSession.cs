namespace CollaborativeCodeEditor.Application.Collaboration.Ports;

public interface ICollaborationSession
{
    Task JoinAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task LeaveAsync(
        Guid documentId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
