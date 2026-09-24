using CollaborativeCodeEditor.Domain.Workspaces;

namespace CollaborativeCodeEditor.Application.Workspaces.Ports;

public interface IWorkspaceRepository
{
    Task AddAsync(
        Workspace workspace,
        CancellationToken cancellationToken = default);

    Task<Workspace?> GetByIdAsync(
        WorkspaceId id,
        CancellationToken cancellationToken = default);
}