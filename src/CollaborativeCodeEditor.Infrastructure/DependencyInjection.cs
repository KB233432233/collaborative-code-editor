using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;
using CollaborativeCodeEditor.Infrastructure.Authentication;
using CollaborativeCodeEditor.Application.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace CollaborativeCodeEditor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "DefaultConnection"));
        });

        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services
            .AddIdentity<ApplicationIdentityUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<
            IIdentityService,
            IdentityService>();

        return services;
    }
}