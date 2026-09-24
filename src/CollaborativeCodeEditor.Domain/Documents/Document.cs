using CollaborativeCodeEditor.Domain.Common;
using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Documents.Events;

namespace CollaborativeCodeEditor.Domain.Documents;

public sealed class Document : AggregateRoot<DocumentId>
{
    public ProjectId ProjectId { get; }

    public DocumentName Name { get; private set; }

    public ProgrammingLanguage Language { get; private set; }

    public string Content { get; private set; }

    public DocumentVersion Version { get; private set; }

    private Document(
        DocumentId id,
        ProjectId projectId,
        DocumentName name,
        ProgrammingLanguage language,
        string content)
        : base(id)
    {
        ProjectId = projectId;
        Name = name;
        Language = language;
        Content = content;
        Version = DocumentVersion.Initial;
    }

    public static Document Create(
    DocumentId id,
    ProjectId projectId,
    DocumentName name,
    ProgrammingLanguage language,
    string content = "")
    {
        if (id.IsEmpty)
            throw new ArgumentException(
                "Document ID cannot be empty.",
                nameof(id));

        if (projectId.IsEmpty)
            throw new ArgumentException(
                "Project ID cannot be empty.",
                nameof(projectId));

        content ??= string.Empty;

        var document = new Document(
            id,
            projectId,
            name,
            language,
            content);

        document.AddDomainEvent(
            new DocumentCreated(
                document.Id,
                document.ProjectId,
                document.Language));

        return document;
    }

    public void Rename(DocumentName name)
    {
        if (Name == name)
            return;

        Name = name;
    }

    public void ChangeLanguage(
    ProgrammingLanguage language)
    {
        if (Language == language)
            return;

        Language = language;
    }

    public void UpdateContent(
    string content,
    DocumentVersion expectedVersion)
    {
        if (content is null)
        {
            throw new ArgumentNullException(
                nameof(content));
        }

        if (Version != expectedVersion)
        {
            throw new InvalidOperationException(
                "Document version conflict.");
        }

        if (Content == content)
            return;

        Content = content;
        Version = Version.Next();
    }
}