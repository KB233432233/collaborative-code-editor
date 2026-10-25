namespace CollaborativeCodeEditor.Api.Contracts.Documents;

public sealed record CreateDocumentRequest(
    string Name,
    int Language,
    string? Content = null);