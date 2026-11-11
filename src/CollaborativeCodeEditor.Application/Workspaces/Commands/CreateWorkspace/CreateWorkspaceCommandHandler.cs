using MediatR;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Common.Time;

namespace CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;

public sealed class CreateWorkspaceCommandHandler
    : IRequestHandler<
        CreateWorkspaceCommand,
        Result<Guid>>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    private readonly IClock _clock;

    public CreateWorkspaceCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        IClock clock)
    {
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<Result<Guid>> Handle(
    CreateWorkspaceCommand command,
    CancellationToken cancellationToken)
    {
        var workspace = Workspace.Create(
            WorkspaceId.New(),
            WorkspaceName.Create(command.Name),
            _currentUser.UserId,
            _clock.UtcNow);

        await _workspaceRepository.AddAsync(
            workspace,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            workspace.Id.Value);
    }
}