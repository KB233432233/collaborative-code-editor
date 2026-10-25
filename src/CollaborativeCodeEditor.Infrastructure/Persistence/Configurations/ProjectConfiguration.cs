using CollaborativeCodeEditor.Domain.Projects;
using CollaborativeCodeEditor.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration
    : IEntityTypeConfiguration<Project>
{
    public void Configure(
        EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ProjectId(value));

        builder.Property(x => x.WorkspaceId)
            .HasConversion(
                id => id.Value,
                value => new WorkspaceId(value))
            .IsRequired();

        builder.Property(x => x.Name)
            .HasConversion(
                name => name.Value,
                value => ProjectName.Create(value))
            .HasMaxLength(ProjectName.MaxLength)
            .IsRequired();

        builder.HasIndex(x => new { x.WorkspaceId, x.Name })
            .IsUnique();

        // Project and Workspace are separate aggregate roots, linked by id.
        // A foreign key keeps orphans out without making Project a child
        // entity of the Workspace aggregate.
        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}