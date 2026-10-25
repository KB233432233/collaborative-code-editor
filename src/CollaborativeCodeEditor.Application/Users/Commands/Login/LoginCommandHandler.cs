using CollaborativeCodeEditor.Application.Authentication;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Application.Common.Persistence;
using MediatR;

namespace CollaborativeCodeEditor.Application.Users.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _jwtTokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        IUserRepository userRepository,
        ITokenService jwtTokenService)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<string>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var identityResult =
            await _identityService.ValidateCredentialsAsync(
                email,
                command.Password,
                cancellationToken);

        if (identityResult.IsFailure)
            return Result<string>.Failure(
                identityResult.Error!);

        var userId = identityResult.Value;

        var user =
            await _userRepository.GetByIdAsync(
                userId,
                cancellationToken);

        if (user is null)
        {
            return Result<string>.Failure(
                new Error(
                    "User.NotFound",
                    "User was not found."));
        }

        var token = _jwtTokenService.CreateToken(
            user.Id,
            user.Email.Value);

        return Result<string>.Success(token);
    }
}