using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(x => x.DisplayName)
            .HasConversion(
                displayName => displayName.Value,
                value => DisplayName.Create(value))
            .HasMaxLength(DisplayName.MaxLength)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasMaxLength(Email.MaxLength)
            .IsRequired();
    }
}