namespace CollaborativeCodeEditor.Application.Documents.Queries.GetDocument;

public sealed record GetDocumentResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    int Language,
    long Version,
    string Content);