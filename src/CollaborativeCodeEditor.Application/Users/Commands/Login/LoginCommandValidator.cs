using FluentValidation;

namespace CollaborativeCodeEditor.Application.Users.Commands.Login;

public sealed class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Email must be a valid email address.")
            .MaximumLength(256)
                .WithMessage(
                    "Email cannot exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.");
    }
}