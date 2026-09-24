using CollaborativeCodeEditor.Application;
using CollaborativeCodeEditor.Application.Common.Authentication;
using CollaborativeCodeEditor.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CollaborativeCodeEditor.Application.Tests.Common;

public static class TestServiceProvider
{
    public static ServiceProvider Create(
        string connectionString,
        ICurrentUser currentUser)
    {
        var services = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] =
                        connectionString
                })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddLogging();

        services.AddApplication();

        services.AddInfrastructure(configuration);

        services.AddScoped<ICurrentUser>(
            _ => currentUser);

        return services.BuildServiceProvider();
    }
}