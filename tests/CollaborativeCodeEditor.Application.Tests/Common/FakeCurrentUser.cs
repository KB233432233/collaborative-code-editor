using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Application.Tests.Common;

public sealed class FakeCurrentUser : ICurrentUser
{
    public FakeCurrentUser(UserId userId)
    {
        UserId = userId;
    }

    public UserId UserId { get; }
}