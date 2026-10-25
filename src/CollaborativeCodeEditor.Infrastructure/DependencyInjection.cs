using CollaborativeCodeEditor.Application.Common.Persistence;
using CollaborativeCodeEditor.Application.Workspaces.Ports;
using CollaborativeCodeEditor.Application.Users.Ports;
using CollaborativeCodeEditor.Infrastructure.Persistence;
using CollaborativeCodeEditor.Infrastructure.Persistence.Repositories;
using CollaborativeCodeEditor.Infrastructure.Authentication;
using CollaborativeCodeEditor.Application.Authentication;
using CollaborativeCodeEditor.Application.Projects.Ports;
using CollaborativeCodeEditor.Application.Documents.Ports;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<IDocumentRepository, DocumentRepository>();

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // services
        //     .AddIdentity<ApplicationIdentityUser, IdentityRole<Guid>>(options =>
        //     {
        //         options.User.RequireUniqueEmail = true;

        //         options.Password.RequiredLength = 8;
        //         options.Password.RequireDigit = true;
        //         options.Password.RequireUppercase = true;
        //         options.Password.RequireLowercase = true;
        //         options.Password.RequireNonAlphanumeric = true;
        //     })
        //     .AddEntityFrameworkStores<AppDbContext>()
        //     .AddDefaultTokenProviders();

        services
           .AddIdentityCore<ApplicationIdentityUser>(options =>
           {
               options.User.RequireUniqueEmail = true;

               options.Password.RequiredLength = 8;
               options.Password.RequireDigit = true;
               options.Password.RequireUppercase = true;
               options.Password.RequireLowercase = true;
               options.Password.RequireNonAlphanumeric = true;

               // Failed attempts are counted in
               // IdentityService.ValidateCredentialsAsync.
               options.Lockout.AllowedForNewUsers = true;
               options.Lockout.MaxFailedAccessAttempts = 5;
               options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
           })
           .AddRoles<IdentityRole<Guid>>()
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders();

        services.AddScoped<
            IIdentityService,
            IdentityService>();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        // services.AddScoped<JwtTokenService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtOptions =
            configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration is missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

        return services;
    }
}