using CollaborativeCodeEditor.Application.Common.Results;
using MediatR;


namespace CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;

public sealed record RegisterNewUserCommand(
    string DisplayName,
    string Email,
    string Password
) : IRequest<Result<Guid>>;