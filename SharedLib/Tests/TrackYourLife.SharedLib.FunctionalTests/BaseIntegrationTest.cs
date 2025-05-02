using MassTransit.Futures.Contracts;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Infrastructure.Data;

namespace TrackYourLife.SharedLib.FunctionalTests;

public abstract class BaseIntegrationTest : IAsyncLifetime
{
    protected bool _disposed;
    protected readonly FunctionalTestWebAppFactory _factory;
    protected readonly HttpClient _client;
    protected readonly IServiceScope _scope;
    protected readonly NutritionWriteDbContext _nutritionWriteDbContext;
    protected readonly NutritionReadDbContext _nutritionReadDbContext;
    protected readonly UsersWriteDbContext _usersWriteDbContext;
    protected readonly UsersReadDbContext _usersReadDbContext;
    protected readonly CommonWriteDbContext _commonWriteDbContext;
    protected readonly CommonReadDbContext _commonReadDbContext;
    protected readonly string _authToken;
    protected readonly UserDto _user;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _scope.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected static async Task CleanupDbSet<T>(DbSet<T> dbSet)
        where T : class
    {
        await dbSet.ExecuteDeleteAsync();
    }

    protected BaseIntegrationTest(
        FunctionalTestWebAppFactory factory,
        FunctionalTestCollection collection
    )
    {
        _factory = factory;
        collection.Should().NotBeNull("Collection should be initialized");

        _client = collection.GetClient();
        _authToken = collection.GetAuthToken();
        _user = collection.GetUser();

        _scope = factory.Services.CreateScope();

        _nutritionWriteDbContext =
            _scope.ServiceProvider.GetRequiredService<NutritionWriteDbContext>();

        _nutritionReadDbContext =
            _scope.ServiceProvider.GetRequiredService<NutritionReadDbContext>();

        _usersWriteDbContext = _scope.ServiceProvider.GetRequiredService<UsersWriteDbContext>();

        _usersReadDbContext = _scope.ServiceProvider.GetRequiredService<UsersReadDbContext>();

        _commonWriteDbContext = _scope.ServiceProvider.GetRequiredService<CommonWriteDbContext>();

        _commonReadDbContext = _scope.ServiceProvider.GetRequiredService<CommonReadDbContext>();
    }

    public ITestHarness GetTestHarness()
    {
        return _factory.Services.GetTestHarness();
    }

    protected abstract Task CleanupDatabaseAsync();

    public virtual async Task InitializeAsync()
    {
        await CleanupDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }
}
