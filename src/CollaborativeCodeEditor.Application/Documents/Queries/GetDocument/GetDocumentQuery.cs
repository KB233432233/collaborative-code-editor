using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Queries.GetDocument;

public sealed record GetDocumentQuery(
    Guid ProjectId,
    Guid DocumentId
) : IRequest<Result<GetDocumentResponse>>;