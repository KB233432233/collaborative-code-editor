using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Documents.Ports;
using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Commands.UpdateDocument;

public sealed class UpdateDocumentCommandHandler
    : IRequestHandler<UpdateDocumentCommand, Result<long>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateDocumentCommandHandler(
        IDocumentRepository documentRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _documentRepository = documentRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<long>> Handle(
        UpdateDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var documentId = new DocumentId(command.DocumentId);

        var document = await _documentRepository.GetByIdAsync(
            documentId,
            cancellationToken);

        if (document is null ||
            document.ProjectId.Value != command.ProjectId)
        {
            return Result<long>.Failure(
                DocumentErrors.NotFound(command.DocumentId));
        }

        var project = await _projectRepository.GetByIdAsync(
            document.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<long>.Failure(
                DocumentErrors.ProjectNotFound(command.ProjectId));
        }

        var workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<long>.Failure(
                DocumentErrors.ProjectNotFound(command.ProjectId));
        }

        if (!workspace.CanBeEditedBy(_currentUser.UserId))
        {
            return Result<long>.Failure(
                DocumentErrors.WorkspaceAccessDenied);
        }

        var expectedVersion = new DocumentVersion(
            command.ExpectedVersion);

        if (document.Version != expectedVersion)
        {
            return Result<long>.Failure(
                DocumentErrors.ConcurrencyConflict(
                    command.ExpectedVersion,
                    document.Version.Value));
        }

        document.UpdateContent(
            command.Content,
            expectedVersion);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<long>.Success(
            document.Version.Value);
    }
}