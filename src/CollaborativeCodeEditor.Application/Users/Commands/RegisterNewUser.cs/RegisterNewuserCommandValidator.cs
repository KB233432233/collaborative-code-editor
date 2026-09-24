using FluentValidation;

namespace CollaborativeCodeEditor.Application.Users.Commands.RegisterNewUser;


public sealed class RegisterNewUserCommandValidator
    : AbstractValidator<RegisterNewUserCommand>
{
    public RegisterNewUserCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
                .WithMessage("Display name is required.")
            .MaximumLength(50)
                .WithMessage(
                    "Display name cannot exceed 50 characters.");
    }
}