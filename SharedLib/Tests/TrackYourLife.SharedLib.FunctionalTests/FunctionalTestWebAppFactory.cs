using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using TrackYourLife.App;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Data;

namespace TrackYourLife.SharedLib.FunctionalTests;

public abstract class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbcontainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithUsername("Nutrition")
        .WithPassword("postgres")
        .WithDatabase("postgres")
        .WithPortBinding(5432, true)
        .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
        .Build();

    private bool _isContainerStarted;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(
            (context, config) =>
            {
                var projectDir = Directory.GetCurrentDirectory();

                config
                    .SetBasePath(projectDir)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile("appsettings.Development.json", optional: false)
                    .AddJsonFile("appsettings.Users.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(
                        "appsettings.Nutrition.json",
                        optional: false,
                        reloadOnChange: true
                    )
                    .AddJsonFile(
                        "appsettings.Nutrition.Testing.json",
                        optional: false,
                        reloadOnChange: true
                    )
                    .AddEnvironmentVariables();
            }
        );

        builder.ConfigureTestServices(async services =>
        {
            if (!_isContainerStarted)
            {
                await _dbcontainer.StartAsync();
                _isContainerStarted = true;
            }

            // Remove all existing DbContext registrations
            var descriptors = services
                .Where(d =>
                    d.ServiceType.BaseType == typeof(DbContextOptions)
                    || d.ServiceType.BaseType == typeof(DbContextOptions)
                )
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Get the configuration
            var configuration = services
                .BuildServiceProvider()
                .GetRequiredService<IConfiguration>();

            // Set the connection string in configuration
            configuration["ConnectionStrings:Database"] = _dbcontainer.GetConnectionString();

            // Register DbContexts with configuration
            services.AddDbContext<NutritionWriteDbContext>();
            services.AddDbContext<NutritionReadDbContext>();
            services.AddDbContext<UsersReadDbContext>();
            services.AddDbContext<UsersWriteDbContext>();
            services.AddDbContext<CommonWriteDbContext>();
            services.AddDbContext<CommonReadDbContext>();

            services.AddEndpointsApiExplorer();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbcontainer.StartAsync();
        _isContainerStarted = true;
    }

    public new async Task DisposeAsync()
    {
        if (_isContainerStarted)
        {
            await _dbcontainer.StopAsync();
        }
    }
}
