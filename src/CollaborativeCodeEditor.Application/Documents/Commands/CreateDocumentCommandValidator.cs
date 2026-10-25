using CollaborativeCodeEditor.Domain.Documents;
using FluentValidation;

namespace CollaborativeCodeEditor.Application.Documents.Commands.CreateDocument;

public sealed class CreateDocumentCommandValidator
    : AbstractValidator<CreateDocumentCommand>
{
    // Guards against a single request uploading an enormous payload.
    private const int MaxContentLength = 1_000_000;

    public CreateDocumentCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
                .WithMessage("Project ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Document name is required.")
            .MaximumLength(DocumentName.MaxLength)
                .WithMessage(
                    $"Document name cannot exceed {DocumentName.MaxLength} characters.");

        RuleFor(x => x.Language)
            .Must(language => Enum.IsDefined(
                typeof(ProgrammingLanguage),
                language))
                .WithMessage("Language is not supported.");

        RuleFor(x => x.Content)
            .MaximumLength(MaxContentLength)
                .WithMessage(
                    $"Document content cannot exceed {MaxContentLength} characters.");
    }
}