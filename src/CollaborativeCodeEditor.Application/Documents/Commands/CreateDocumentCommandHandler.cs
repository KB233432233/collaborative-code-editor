using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Documents.Ports;
using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Workspaces;
using MediatR;

namespace CollaborativeCodeEditor.Application.Documents.Commands.CreateDocument;

public sealed class CreateDocumentCommandHandler
    : IRequestHandler<CreateDocumentCommand, Result<Guid>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateDocumentCommandHandler(
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

    public async Task<Result<Guid>> Handle(
        CreateDocumentCommand command,
        CancellationToken cancellationToken)
    {
        var projectId = new ProjectId(command.ProjectId);

        var project = await _projectRepository.GetByIdAsync(
            projectId,
            cancellationToken);

        if (project is null)
        {
            return Result<Guid>.Failure(
                DocumentErrors.ProjectNotFound(command.ProjectId));
        }

        // Authorization walks the full chain: the document belongs to the
        // project, and the project belongs to a workspace whose members are
        // the only people allowed to write.
        var workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<Guid>.Failure(
                DocumentErrors.ProjectNotFound(command.ProjectId));
        }

        if (!workspace.CanBeEditedBy(_currentUser.UserId))
        {
            return Result<Guid>.Failure(
                DocumentErrors.WorkspaceAccessDenied);
        }

        if (await _documentRepository.ExistsWithNameAsync(
                projectId,
                command.Name,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                DocumentErrors.NameAlreadyExists);
        }

        var document = Document.Create(
            DocumentId.New(),
            projectId,
            DocumentName.Create(command.Name),
            (ProgrammingLanguage)command.Language,
            command.Content ?? string.Empty);

        await _documentRepository.AddAsync(
            document,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(document.Id.Value);
    }
}