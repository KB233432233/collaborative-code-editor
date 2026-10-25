using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Projects.AddAsync(
            project,
            cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(
        ProjectId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Projects
            .FirstOrDefaultAsync(
                project => project.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(
        WorkspaceId workspaceId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var normalized = ProjectName.Create(name);

        return await _dbContext.Projects
            .AnyAsync(
                project => project.WorkspaceId == workspaceId
                    && project.Name == normalized,
                cancellationToken);
    }
}