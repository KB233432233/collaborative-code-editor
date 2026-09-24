using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;

public sealed class WorkspaceRepository : IWorkspaceRepository
{
    private readonly AppDbContext _dbContext;

    public WorkspaceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Workspace workspace,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Workspaces.AddAsync(
            workspace,
            cancellationToken);
    }

    public async Task<Workspace?> GetByIdAsync(
        WorkspaceId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Workspaces
            .Include(x => x.Members)
            .FirstOrDefaultAsync(
                workspace => workspace.Id == id,
                cancellationToken);
    }
}