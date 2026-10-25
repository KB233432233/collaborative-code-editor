using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Workspaces;
using MediatR;

namespace CollaborativeCodeEditor.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var workspaceId = new WorkspaceId(command.WorkspaceId);

        // Tracked include of Members, so the role check below is
        // answered without a second round trip.
        var workspace = await _workspaceRepository.GetByIdAsync(
            workspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<Guid>.Failure(
                ProjectErrors.WorkspaceNotFound(command.WorkspaceId));
        }

        if (!workspace.CanBeEditedBy(_currentUser.UserId))
        {
            return Result<Guid>.Failure(
                ProjectErrors.WorkspaceAccessDenied);
        }

        if (await _projectRepository.ExistsWithNameAsync(
                workspaceId,
                command.Name,
                cancellationToken))
        {
            return Result<Guid>.Failure(
                new Error(
                    "Project.NameAlreadyExists",
                    "A project with this name already exists in the workspace."));
        }

        var project = Project.Create(
            ProjectId.New(),
            workspaceId,
            ProjectName.Create(command.Name));

        await _projectRepository.AddAsync(
            project,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(project.Id.Value);
    }
}