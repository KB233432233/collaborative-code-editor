using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;

namespace CollaborativeCodeEditor.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    Guid WorkspaceId,
    string Name
) : IRequest<Result<Guid>>;