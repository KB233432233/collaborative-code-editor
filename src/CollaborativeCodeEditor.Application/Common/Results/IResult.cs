namespace CollaborativeCodeEditor.Application.Common.Results;

public interface IResult
{
    bool IsSuccess { get; }

    Error? Error { get; }
}