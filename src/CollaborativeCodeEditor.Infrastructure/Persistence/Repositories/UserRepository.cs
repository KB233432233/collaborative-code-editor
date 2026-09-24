using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;


public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default
    )
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        UserId id,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                user => user.Id == id,
                cancellationToken);
    }
}