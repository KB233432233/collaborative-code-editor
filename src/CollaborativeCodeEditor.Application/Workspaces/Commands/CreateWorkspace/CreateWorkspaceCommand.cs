using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;

namespace CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;

public sealed record CreateWorkspaceCommand(
    string Name
) : IRequest<Result<Guid>>;

