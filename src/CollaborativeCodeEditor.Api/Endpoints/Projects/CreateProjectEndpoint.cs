using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Api.Contracts.Projects;
using CollaborativeCodeEditor.Application.Projects.Commands.CreateProject;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Projects;

public static class CreateProjectEndpoint
{
    public static IEndpointRouteBuilder MapCreateProject(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/workspaces/{workspaceId:guid}/projects",
            async (
                Guid workspaceId,
                CreateProjectRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateProjectCommand(
                    workspaceId,
                    request.Name);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Created(
                    $"/api/workspaces/{workspaceId}/projects/{result.Value}",
                    new CreateProjectResponse(result.Value));
            })
            .RequireAuthorization();

        return endpoints;
    }
}