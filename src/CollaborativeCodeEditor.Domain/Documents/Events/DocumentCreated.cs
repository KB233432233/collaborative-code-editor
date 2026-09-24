using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Projects;

namespace CollaborativeCodeEditor.Domain.Documents.Events;

public sealed record DocumentCreated(
    DocumentId DocumentId,
    ProjectId ProjectId,
    ProgrammingLanguage Language
) : DomainEvent;