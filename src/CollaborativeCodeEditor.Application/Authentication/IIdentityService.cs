using CollaborativeCodeEditor.Domain.Users;
using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Application.Authentication;

public interface IIdentityService
{
    Task<Result<UserId>> CreateUserAsync(
        Email email,
        string password,
        CancellationToken cancellationToken = default);
}