using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Application.Users.Ports;


public interface IUserRepository
{
    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        UserId id,
        CancellationToken cancellationToken = default);
}