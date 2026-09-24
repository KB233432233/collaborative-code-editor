namespace CollaborativeCodeEditor.Application.Common.Results;

public sealed record Error(
    string Code,
    string Description);