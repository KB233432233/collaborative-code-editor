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
            .MaximumLength(100)
                .WithMessage(
                    "Display name cannot exceed 100 characters.");
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Email must be a valid email address.");
    }
}