using ApplicationResult =
    CollaborativeCodeEditor.Application.Common.Results.Result;

namespace CollaborativeCodeEditor.Api.Common.Results;

public static class ResultExtensions
{
    public static Microsoft.AspNetCore.Http.IResult ToProblemDetails(
        this ApplicationResult result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException(
                "Cannot convert a successful result to a problem.");
        }

        return Microsoft.AspNetCore.Http.Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: result.Error!.Code,
            detail: result.Error.Description);
    }
}