using FluentValidation;

namespace CollaborativeCodeEditor.Application.Documents.Commands.UpdateDocument;

public sealed class UpdateDocumentCommandValidator
    : AbstractValidator<UpdateDocumentCommand>
{
    private const int MaxContentLength = 1_000_000;

    public UpdateDocumentCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.DocumentId)
            .NotEmpty();

        RuleFor(x => x.Content)
            .NotNull()
            .MaximumLength(MaxContentLength);

        RuleFor(x => x.ExpectedVersion)
            .GreaterThanOrEqualTo(0);
    }
}