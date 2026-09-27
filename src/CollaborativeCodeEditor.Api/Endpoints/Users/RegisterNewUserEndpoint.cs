using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Api.Contracts.Users;
using CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Users;

public static class RegisterNewUserEndpoint
{
    public static IEndpointRouteBuilder MapRegisterNewUser(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/users",
            async (
                RegisterNewUserRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new RegisterNewUserCommand(
                        request.DisplayName,
                        request.Email);

                var result =
                    await sender.Send(
                        command,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Created(
                    $"/api/users/{result.Value}",
                    new RegisterNewUserResponse(
                        result.Value));
            });

        return endpoints;
    }
}