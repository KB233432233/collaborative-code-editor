using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Commands.UpdateDocument;

public sealed record UpdateDocumentCommand(
    Guid ProjectId,
    Guid DocumentId,
    string Content,
    long ExpectedVersion
) : IRequest<Result<long>>;