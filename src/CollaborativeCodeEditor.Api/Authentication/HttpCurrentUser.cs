using System.Security.Claims;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Domain.Users;

namespace CollaborativeCodeEditor.Api.Authentication;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public UserId UserId
    {
        get
        {
            var userId = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var id))
            {
                throw new UnauthorizedAccessException(
                    "The current user is not authenticated.");
            }

            return new UserId(id);
        }
    }
}