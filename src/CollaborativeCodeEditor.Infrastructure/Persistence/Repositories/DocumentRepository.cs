using CollaborativeCodeEditor.Application.Documents.Ports;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _dbContext;

    public DocumentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Documents.AddAsync(
            document,
            cancellationToken);
    }

    public async Task<Document?> GetByIdAsync(
        DocumentId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Documents
            .FirstOrDefaultAsync(
                document => document.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(
        ProjectId projectId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Compare the converted value object, not Name.Value: reaching into
        // .Value inside a predicate is not translatable to SQL.
        var normalized = DocumentName.Create(name);

        return await _dbContext.Documents
            .AnyAsync(
                document => document.ProjectId == projectId
                    && document.Name == normalized,
                cancellationToken);
    }
}