using CollaborativeCodeEditor.Api.Common.Results;
using CollaborativeCodeEditor.Api.Contracts.Documents;
using CollaborativeCodeEditor.Application.Documents.Commands.UpdateDocument;
using MediatR;

namespace CollaborativeCodeEditor.Api.Endpoints.Documents;

public static class UpdateDocumentEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDocument(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut(
            "/api/projects/{projectId:guid}/documents/{documentId:guid}",
            async (
                Guid projectId,
                Guid documentId,
                UpdateDocumentRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateDocumentCommand(
                    projectId,
                    documentId,
                    request.Content,
                    request.ExpectedVersion);

                var result = await sender.Send(
                    command,
                    cancellationToken);

                if (result.IsFailure)
                {
                    return result.ToProblemDetails();
                }

                return Results.Ok(
                    new UpdateDocumentResponse(result.Value));
            })
            .RequireAuthorization();

        return endpoints;
    }
}