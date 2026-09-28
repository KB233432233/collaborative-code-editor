using Microsoft.AspNetCore.Identity;

namespace CollaborativeCodeEditor.Infrastructure.Authentication;

public sealed class ApplicationIdentityUser
    : IdentityUser<Guid>
{
}