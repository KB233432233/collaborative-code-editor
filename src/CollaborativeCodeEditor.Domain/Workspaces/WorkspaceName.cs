using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Workspaces;

public sealed record WorkspaceName : ValueObject
{
    public const int MaxLength = 100;

    public string Value { get; }

    private WorkspaceName(string value)
    {
        Value = value;
    }

    public static WorkspaceName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Workspace name cannot be empty.",
                nameof(value));
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Workspace name cannot exceed {MaxLength} characters.",
                nameof(value));
        }

        return new WorkspaceName(normalizedValue);
    }

    public override string ToString()
        => Value;
}