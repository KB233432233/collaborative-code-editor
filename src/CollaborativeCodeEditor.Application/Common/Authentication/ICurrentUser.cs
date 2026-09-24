using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Application.Common.Authentication;

public interface ICurrentUser
{
    UserId UserId { get; }
}