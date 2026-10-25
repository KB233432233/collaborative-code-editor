using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Application.Authentication;

public interface ITokenService
{
    string CreateToken(
        UserId userId,
        string email);
}