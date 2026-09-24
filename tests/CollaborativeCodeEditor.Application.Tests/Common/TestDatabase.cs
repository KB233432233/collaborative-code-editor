using CollaborativeCodeEditor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace CollaborativeCodeEditor.Application.Tests.Common;

public sealed class TestDatabase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("collaborative_code_editor_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    public AppDbContext DbContext { get; private set; } = null!;

    public string ConnectionString =>
        _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        DbContext = new AppDbContext(options);

        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}