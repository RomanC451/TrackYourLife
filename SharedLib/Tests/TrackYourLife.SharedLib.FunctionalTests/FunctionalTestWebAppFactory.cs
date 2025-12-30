using MassTransit;
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
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.Modules.Youtube.Infrastructure.Data;
using WireMock.Server;

namespace TrackYourLife.SharedLib.FunctionalTests;

public abstract class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public WireMockServer WireMockServer = null!;

    protected FunctionalTestWebAppFactory(string name)
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithName(name + "_" + Guid.NewGuid().ToString())
            .WithUsername("FunctionalTests")
            .WithPassword("postgres")
            .WithDatabase("postgres")
            .WithPortBinding(5432, true)
            .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
            .Build();
    }

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
                        "appsettings.Defaults.Testing.json",
                        optional: false,
                        reloadOnChange: true
                    )
                    .AddJsonFile(TestingSettingsFileName, optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables();
            }
        );

        builder.ConfigureTestServices(async services =>
        {
            if (!_isContainerStarted)
            {
                await _dbContainer.StartAsync();
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
            configuration["ConnectionStrings:Database"] = _dbContainer.GetConnectionString();
            configuration["FoodApi:BaseUrl"] = WireMockServer.Urls[0];
            configuration["FoodApi:BaseApiUrl"] = WireMockServer.Urls[0];

            // Register DbContexts with configuration
            services.AddDbContext<NutritionWriteDbContext>();
            services.AddDbContext<NutritionReadDbContext>();
            services.AddDbContext<UsersReadDbContext>();
            services.AddDbContext<UsersWriteDbContext>();
            services.AddDbContext<CommonWriteDbContext>();
            services.AddDbContext<CommonReadDbContext>();
            services.AddDbContext<TrainingsWriteDbContext>();
            services.AddDbContext<TrainingsReadDbContext>();
            services.AddDbContext<YoutubeWriteDbContext>();
            services.AddDbContext<YoutubeReadDbContext>();

            services.AddMassTransitTestHarness();

            services.AddEndpointsApiExplorer();
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        _isContainerStarted = true;
        WireMockServer = WireMockServer.Start();
    }

    public new async Task DisposeAsync()
    {
        if (_isContainerStarted)
        {
            await _dbContainer.StopAsync();
        }
        WireMockServer.Stop();
    }

    public abstract string TestingSettingsFileName { get; }
}
