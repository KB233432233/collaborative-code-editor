using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Documents.Events;
using CollaborativeCodeEditor.Domain.Projects;

namespace CollaborativeCodeEditor.Domain.Tests.Documents;

public sealed class DocumentTests
{
    [Fact]
    public void Create_ShouldCreateDocument()
    {
        // Arrange
        var documentId = DocumentId.New();
        var projectId = ProjectId.New();

        // Act
        var document = Document.Create(
            documentId,
            projectId,
            DocumentName.Create("Program.cs"),
            ProgrammingLanguage.CSharp);

        // Assert
        Assert.Equal(documentId, document.Id);
        Assert.Equal(projectId, document.ProjectId);
        Assert.Equal("Program.cs", document.Name.Value);
        Assert.Equal(
            ProgrammingLanguage.CSharp,
            document.Language);
        Assert.Equal(string.Empty, document.Content);
        Assert.Equal(
            DocumentVersion.Initial,
            document.Version);
    }

    [Fact]
    public void UpdateContent_WithCorrectVersion_ShouldUpdateContent()
    {
        // Arrange
        var document = Document.Create(
            DocumentId.New(),
            ProjectId.New(),
            DocumentName.Create("Program.cs"),
            ProgrammingLanguage.CSharp);

        // Act
        document.UpdateContent(
            "Console.WriteLine(\"Hello\");",
            DocumentVersion.Initial);

        // Assert
        Assert.Equal(
            "Console.WriteLine(\"Hello\");",
            document.Content);

        Assert.Equal(
            new DocumentVersion(1),
            document.Version);
    }
    [Fact]
    public void UpdateContent_WithIncorrectVersion_ShouldThrowConflict()
    {
        // Arrange
        var document = Document.Create(
            DocumentId.New(),
            ProjectId.New(),
            DocumentName.Create("Program.cs"),
            ProgrammingLanguage.CSharp);

        document.UpdateContent(
            "Version 1",
            DocumentVersion.Initial);

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() =>
            document.UpdateContent(
                "Version conflict",
                DocumentVersion.Initial));
    }

    [Fact]
    public void Rename_ShouldChangeName()
    {
        // Arrange
        var document = Document.Create(
            DocumentId.New(),
            ProjectId.New(),
            DocumentName.Create("Old.cs"),
            ProgrammingLanguage.CSharp);

        // Act
        document.Rename(
            DocumentName.Create("New.cs"));

        // Assert
        Assert.Equal(
            "New.cs",
            document.Name.Value);
    }

    [Fact]
    public void ChangeLanguage_ShouldChangeLanguage()
    {
        // Arrange
        var document = Document.Create(
            DocumentId.New(),
            ProjectId.New(),
            DocumentName.Create("Program.cs"),
            ProgrammingLanguage.CSharp);

        // Act
        document.ChangeLanguage(
            ProgrammingLanguage.Python);

        // Assert
        Assert.Equal(
            ProgrammingLanguage.Python,
            document.Language);
    }
}