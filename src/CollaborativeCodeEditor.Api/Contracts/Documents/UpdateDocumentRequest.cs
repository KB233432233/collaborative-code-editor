namespace CollaborativeCodeEditor.Api.Contracts.Documents;

public sealed record UpdateDocumentRequest(
    string Content,
    long ExpectedVersion);