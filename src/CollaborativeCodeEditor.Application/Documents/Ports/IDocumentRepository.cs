using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;

namespace CollaborativeCodeEditor.Application.Documents.Ports;

public interface IDocumentRepository
{
    Task AddAsync(
        Document document,
        CancellationToken cancellationToken = default);

    Task<Document?> GetByIdAsync(
        DocumentId id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsWithNameAsync(
        ProjectId projectId,
        string name,
        CancellationToken cancellationToken = default);
}