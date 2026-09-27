using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Users;

public sealed record Email : ValueObject
{
    public const int MaxLength = 50;
    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Email cannot be empty.",
                nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Email cannot exceed {MaxLength} characters.",
                nameof(value));
        }

        return new Email(normalizedValue);
    }
}