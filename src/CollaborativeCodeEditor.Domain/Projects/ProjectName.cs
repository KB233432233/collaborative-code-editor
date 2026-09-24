using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Projects;

public sealed record ProjectName : ValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private ProjectName(string value)
    {
        Value = value;
    }

    public static ProjectName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Project name cannot be empty.",
                nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Project name cannot exceed {MaxLength} characters.",
                nameof(value));
        }

        return new ProjectName(normalizedValue);
    }

    public override string ToString()
        => Value;
}