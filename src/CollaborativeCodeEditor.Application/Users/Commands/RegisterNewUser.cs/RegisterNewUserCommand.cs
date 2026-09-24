using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;


namespace CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;

public sealed record RegisterNewUserCommand(
    string DisplayName
) : IRequest<Result<Guid>>;