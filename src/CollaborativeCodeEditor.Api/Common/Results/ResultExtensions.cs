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

        // Map by error-code suffix until Error carries an explicit kind.
        var statusCode = result.Error!.Code switch
        {
            var code when code.EndsWith(".NotFound") =>
                StatusCodes.Status404NotFound,

            var code when code.EndsWith(".AccessDenied") =>
                StatusCodes.Status403Forbidden,

            var code when code.EndsWith(".NameAlreadyExists") =>
                StatusCodes.Status409Conflict,

            _ => StatusCodes.Status400BadRequest
        };

        return Microsoft.AspNetCore.Http.Results.Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Description);
    }
}