using CollaborativeCodeEditor.Domain.Documents;
using CollaborativeCodeEditor.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration
    : IEntityTypeConfiguration<Document>
{
    public void Configure(
        EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new DocumentId(value));

        builder.Property(x => x.ProjectId)
            .HasConversion(
                id => id.Value,
                value => new ProjectId(value))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => DocumentName.Create(value))
            .HasMaxLength(DocumentName.MaxLength)
            .IsRequired();

        builder.Property(x => x.Language)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.Version)
            .HasConversion(
                version => version.Value,
                value => new DocumentVersion(value))
            .IsRequired();

        // Optimistic concurrency: a stale Version fails the UPDATE
        // instead of silently overwriting another writer.
        builder.Property(x => x.Version)
            .IsConcurrencyToken();

        builder.HasIndex(x => new { x.ProjectId, x.Name })
            .IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}