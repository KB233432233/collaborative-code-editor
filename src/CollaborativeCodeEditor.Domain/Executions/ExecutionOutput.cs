using CollaborativeCodeEditor.Domain.Common;

namespace CollaborativeCodeEditor.Domain.Executions;

public sealed record ExecutionOutput : ValueObject
{
    public string StandardOutput { get; }

    public string StandardError { get; }

    private ExecutionOutput(
        string standardOutput,
        string standardError)
    {
        StandardOutput = standardOutput;
        StandardError = standardError;
    }

    public static ExecutionOutput Create(
        string? standardOutput,
        string? standardError)
    {
        return new ExecutionOutput(
            standardOutput ?? string.Empty,
            standardError ?? string.Empty);
    }

    public static ExecutionOutput Empty =>
        new(string.Empty, string.Empty);
}