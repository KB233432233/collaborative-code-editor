using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceMemberConfiguration
    : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(
        EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("workspace_members");

        builder.HasKey(x => new
        {
            x.WorkspaceId,
            x.UserId
        });

        builder.Property(x => x.WorkspaceId)
            .HasConversion(
                id => id.Value,
                value => new WorkspaceId(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired();
    }
}