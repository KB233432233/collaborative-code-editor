using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Documents.Ports;
using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Documents;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Queries.GetDocument;

public sealed class GetDocumentQueryHandler
    : IRequestHandler<GetDocumentQuery, Result<GetDocumentResponse>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly ICurrentUser _currentUser;

    public GetDocumentQueryHandler(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        ICurrentUser currentUser)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GetDocumentResponse>> Handle(
        GetDocumentQuery query,
        CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(
            new DocumentId(query.DocumentId),
            cancellationToken);

        if (document is null)
        {
            return Result<GetDocumentResponse>.Failure(
                DocumentErrors.NotFound(query.DocumentId));
        }

        // The URL names the project. A document requested through the wrong
        // project must 404, not reveal that it exists elsewhere.
        if (document.ProjectId.Value != query.ProjectId)
        {
            return Result<GetDocumentResponse>.Failure(
                DocumentErrors.NotFound(query.DocumentId));
        }

        var project = await _projectRepository.GetByIdAsync(
            document.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<GetDocumentResponse>.Failure(
                DocumentErrors.ProjectNotFound(query.ProjectId));
        }

        var workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<GetDocumentResponse>.Failure(
                DocumentErrors.ProjectNotFound(query.ProjectId));
        }

        if (!workspace.CanBeViewedBy(_currentUser.UserId))
        {
            return Result<GetDocumentResponse>.Failure(
                DocumentErrors.WorkspaceAccessDenied);
        }

        return Result<GetDocumentResponse>.Success(
            new GetDocumentResponse(
                document.Id.Value,
                document.ProjectId.Value,
                document.Name.Value,
                (int)document.Language,
                document.Version.Value,
                document.Content));
    }
}