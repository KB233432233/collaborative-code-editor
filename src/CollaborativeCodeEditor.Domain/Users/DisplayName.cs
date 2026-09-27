using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Users;

public sealed record DisplayName : ValueObject
{
    public const int MaxLength = 100;
    public string Value { get; }

    private DisplayName(string value)
    {
        Value = value;
    }

    private static string IsValidDisplayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Display name cannot be empty.",
                nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Display name cannot exceed {MaxLength} characters.",
                nameof(value));
        }

        return normalizedValue;
    }

    public static DisplayName Create(string value)
    {
        var normalizedValue = IsValidDisplayName(value);

        return new DisplayName(normalizedValue);
    }

    public static DisplayName Change(string value)
    {
        var normalizedValue = IsValidDisplayName(value);

        return new DisplayName(normalizedValue);
    }
}