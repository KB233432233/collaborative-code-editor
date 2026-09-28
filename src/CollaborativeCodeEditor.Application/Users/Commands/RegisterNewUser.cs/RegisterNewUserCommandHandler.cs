using CollaborativeCodeEditor.Application.Authentication;
using CollaborativeCodeEditor.Application.Common.Results;
using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Application.Common.Persistence;
using MediatR;

namespace CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;

public sealed class RegisterNewUserCommandHandler
    : IRequestHandler<
        RegisterNewUserCommand,
        Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterNewUserCommandHandler(
        IUserRepository userRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        RegisterNewUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = Email.Create(command.Email);

        var identityResult =
            await _identityService.CreateUserAsync(
                email,
                command.Password,
                cancellationToken);

        if (identityResult.IsFailure)
            return Result<Guid>.Failure(
                identityResult.Error!);

        var user = User.Create(
            identityResult.Value,
            DisplayName.Create(command.DisplayName),
            email);

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            user.Id.Value);
    }
}