using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;
using FluentAssertions;

namespace CollaborativeCodeEditor.Infrastructure.Tests.Persistence;

public sealed class UserRepositoryTests
    : IClassFixture<TestDatabase>
{
    private readonly TestDatabase _database;

    public UserRepositoryTests(TestDatabase database)
    {
        _database = database;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        var repository =
            new UserRepository(_database.DbContext);

        var userId = new UserId(Guid.NewGuid());
        // var userName = UserName.Create("Test User");

        var user = User.Create(
            userId,
            DisplayName.Create("Test User"),
            Email.Create("testuser@example.com"));

        await repository.AddAsync(user);

        await _database.DbContext.SaveChangesAsync();

        var result = await repository.GetByIdAsync(userId);

        result.Should().NotBeNull();

        result!.Id.Should().Be(userId);
        result.Email.Value.Should().Be("testuser@example.com");
        result.DisplayName.Value.Should().Be("Test User");
    }
}