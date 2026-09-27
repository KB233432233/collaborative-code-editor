using MediatR;
// using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;

public sealed class RegisterNewUserCommandHandler
    : IRequestHandler<
        RegisterNewUserCommand,
        Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterNewUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
    RegisterNewUserCommand command,
    CancellationToken cancellationToken)
    {
        var user = User.Create(
            UserId.New(),
           DisplayName.Create(command.DisplayName),
            Email.Create(command.Email));

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            user.Id.Value);
    }
}