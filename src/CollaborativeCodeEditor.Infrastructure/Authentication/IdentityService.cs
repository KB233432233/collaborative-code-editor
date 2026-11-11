using CollaborativeCodeEditor.Application.Authentication;
using CollaborativeCodeEditor.Domain.Users;
using Microsoft.AspNetCore.Identity;
using CollaborativeCodeEditor.Application.Common.Results;

namespace CollaborativeCodeEditor.Infrastructure.Authentication;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly SignInManager<ApplicationIdentityUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationIdentityUser> userManager,
        SignInManager<ApplicationIdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }



    public async Task<Result<UserId>> CreateUserAsync(
        Email email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var identityUser = new ApplicationIdentityUser
        {
            Id = Guid.NewGuid(),
            UserName = email.Value,
            Email = email.Value
        };

        var result = await _userManager.CreateAsync(
            identityUser,
            password);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();

            return Result<UserId>.Failure(
                new Error(
                    $"Identity.{error.Code}",
                    error.Description));
        }

        return Result<UserId>.Success(
            new UserId(identityUser.Id));
    }

    public async Task<Result<UserId>> ValidateCredentialsAsync(
    Email email,
    string password,
    CancellationToken cancellationToken = default)
    {
        var identityUser =
            await _userManager.FindByEmailAsync(email.Value);

        if (identityUser is null)
        {
            return Result<UserId>.Failure(
                new Error(
                    "Identity.InvalidCredentials",
                    "Invalid email or password."));
        }

        var signInResult =
            await _signInManager.CheckPasswordSignInAsync(
                identityUser,
                password,
                lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return Result<UserId>.Failure(
                new Error(
                    "Identity.InvalidCredentials",
                    "Invalid email or password."));
        }

        return Result<UserId>.Success(
            new UserId(identityUser.Id));
    }
}