using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Documents;

public sealed record DocumentName : ValueObject
{
    public const int MaxLength = 255;

    public string Value { get; }

    private DocumentName(string value)
    {
        Value = value;
    }

    public static DocumentName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Document name cannot be empty.",
                nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Document name cannot exceed {MaxLength} characters.",
                nameof(value));
        }

        return new DocumentName(normalizedValue);
    }

    public override string ToString()
        => Value;
}