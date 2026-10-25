using CollaborativeCodeEditor.Application.Users.Commands.Login;
using CollaborativeCodeEditor.Api.Contracts.Users;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Users;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/auth/login",
            async (
                LoginRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {

                var command = new LoginCommand(
                    request.Email,
                    request.Password);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                    return Results.Unauthorized();

                return Results.Ok(new
                {
                    accessToken = result.Value
                });
            });

        return endpoints;
    }
}