using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Application.Documents.Queries.GetDocument;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Documents;

public static class GetDocumentEndpoint
{
    public static IEndpointRouteBuilder MapGetDocument(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/projects/{projectId:guid}/documents/{documentId:guid}",
            async (
                Guid projectId,
                Guid documentId,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetDocumentQuery(
                        projectId,
                        documentId),
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Ok(result.Value);
            })
            .RequireAuthorization();

        return endpoints;
    }
}