using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Commands.CreateDocument;

public sealed record CreateDocumentCommand(
    Guid ProjectId,
    string Name,
    int Language,
    string? Content
) : IRequest<Result<Guid>>;