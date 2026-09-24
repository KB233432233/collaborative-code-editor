namespace CollaborativeCodeEditor.Application.Common.Results;

public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(
        T value)
        : base(true, null)
    {
        Value = value;
    }

    private Result(
        Error error)
        : base(false, error)
    {
    }

    public static Result<T> Success(T value)
        => new(value);

    public static Result<T> Failure(Error error)
        => new(error);
}