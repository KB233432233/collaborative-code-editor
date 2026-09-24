using FluentValidation;

namespace CollaborativeCodeEditor.Application.Workspaces.Commands.CreateWorkspace;

public sealed class CreateWorkspaceCommandValidator
    : AbstractValidator<CreateWorkspaceCommand>
{
    public CreateWorkspaceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Workspace name is required.")
            .MaximumLength(100)
                .WithMessage(
                    "Workspace name cannot exceed 100 characters.");
    }
}

