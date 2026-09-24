namespace CollaborativeCodeEditor.Domain.Documents;

public readonly record struct DocumentVersion(long Value)
{
    public static DocumentVersion Initial =>
        new(0);

    public DocumentVersion Next()
        => new(Value + 1);
}