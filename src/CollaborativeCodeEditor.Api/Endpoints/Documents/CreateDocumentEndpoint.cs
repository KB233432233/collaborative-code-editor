using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Api.Contracts.Documents;
using CollaborativeCodeEditor.Application.Documents.Commands.CreateDocument;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Documents;

public static class CreateDocumentEndpoint
{
    public static IEndpointRouteBuilder MapCreateDocument(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost(
            "/api/projects/{projectId:guid}/documents",
            async (
                Guid projectId,
                CreateDocumentRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateDocumentCommand(
                    projectId,
                    request.Name,
                    request.Language,
                    request.Content);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Created(
                    $"/api/projects/{projectId}/documents/{result.Value}",
                    new CreateDocumentResponse(result.Value));
            })
            .RequireAuthorization();

        return endpoints;
    }
}