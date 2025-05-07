using System.Net;
using System.Net.Http.Json;
using Bogus;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

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
    protected string _authToken;
    protected UserDto _user;
    protected string _deviceId;
    protected string _userPassword;

    protected string _userEmail;

    private readonly FunctionalTestCollection _collection;

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

    public HttpClient CreateUnauthorizedClient() => _collection.CreateUnauthorizedClient();

    protected BaseIntegrationTest(
        FunctionalTestWebAppFactory factory,
        FunctionalTestCollection collection
    )
    {
        _factory = factory;
        collection.Should().NotBeNull("Collection should be initialized");

        _collection = collection;

        _client = collection.GetClient();
        _authToken = collection.GetAuthToken();
        _user = collection.GetUser();

        _deviceId = collection.GetDeviceId();

        _userPassword = collection.GetUserPassword();

        _userEmail = collection.GetUserEmail();

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

    protected static async Task WaitForOutboxEventsToBeHandledAsync(
        DbSet<OutboxMessage> dbSet,
        CancellationToken cancellationToken = default
    )
    {
        var timeout = TimeSpan.FromSeconds(20);
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var hasUnprocessedEvents = await dbSet.AnyAsync(
                m => !m.ProcessedOnUtc.HasValue,
                cancellationToken
            );

            if (!hasUnprocessedEvents)
            {
                return;
            }

            await Task.Delay(100, cancellationToken);
        }

        throw new TimeoutException("Timeout waiting for outbox events to be processed");
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

public static class HttpClientExtensions
{
    private static readonly Faker _faker = new();

    public static async Task<UserDto> RegisterAndLoginNewUserAsync(
        this HttpClient client,
        string? email = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        DeviceId? deviceId = null
    )
    {
        var request = new
        {
            Email = email ?? _faker.Internet.Email(),
            Password = password ?? $"{_faker.Internet.Password()}123!",
            FirstName = firstName ?? _faker.Person.FirstName,
            LastName = lastName ?? _faker.Person.LastName,
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", request);
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        var loginRequest = new
        {
            Email = request.Email,
            Password = request.Password,
            DeviceId = deviceId?.ToString() ?? DeviceId.NewId().ToString(),
        };

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var tokenResponse = await loginResponse.ShouldHaveStatusCodeAndContent<TokenResponse>(
            HttpStatusCode.OK
        );

        tokenResponse.Should().NotBeNull();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                tokenResponse!.TokenValue
            );

        var userResponse = await client.GetAsync("/api/users/me");
        var user = await userResponse.ShouldHaveStatusCodeAndContent<UserDto>(HttpStatusCode.OK);

        user.Should().NotBeNull();

        return user!;
    }
}
