using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Domain.Workspaces;
using CollaborativeCodeEditor.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence;

public sealed class AppDbContext
    : IdentityDbContext<
        ApplicationIdentityUser,
        IdentityRole<Guid>,
        Guid>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }
}