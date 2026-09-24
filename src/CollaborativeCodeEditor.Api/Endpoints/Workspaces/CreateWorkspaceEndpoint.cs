using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Api.Contracts.Workspaces;
using CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Workspaces;

public static class CreateWorkspaceEndpoint
{
    public static IEndpointRouteBuilder MapCreateWorkspace(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/workspaces",
            async (
                CreateWorkspaceRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command =
                    new CreateWorkspaceCommand(
                        request.Name);

                var result =
                    await sender.Send(
                        command,
                        cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Created(
                    $"/api/workspaces/{result.Value}",
                    new CreateWorkspaceResponse(
                        result.Value));
            });

        return endpoints;
    }
}