using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Workspaces;

namespace CollaborativeCodeEditor.Application.Projects.Ports;

public interface IProjectRepository
{
    Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(
        ProjectId id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsWithNameAsync(
        WorkspaceId workspaceId,
        string name,
        CancellationToken cancellationToken = default);
}