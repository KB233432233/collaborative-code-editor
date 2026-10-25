using MediatR;
using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Users.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<string>>;