using CollaborativeCodeEditor.Domain.Projects;
using FluentValidation;

namespace CollaborativeCodeEditor.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandValidator
    : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.WorkspaceId)
            .NotEmpty()
                .WithMessage("Workspace ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Project name is required.")
            .MaximumLength(ProjectName.MaxLength)
                .WithMessage(
                    $"Project name cannot exceed {ProjectName.MaxLength} characters.");
    }
}