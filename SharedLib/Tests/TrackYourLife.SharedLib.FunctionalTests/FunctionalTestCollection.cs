using System.Net;
using System.Net.Http.Json;
using TrackYourLife.Modules.Users.Contracts.Users;

namespace TrackYourLife.SharedLib.FunctionalTests;

public abstract class FunctionalTestCollection : IAsyncLifetime
{
    private readonly FunctionalTestWebAppFactory _factory;
    private HttpClient _client = null!;
    private string _authToken = null!;
    private UserDto _user = null!;

    protected FunctionalTestCollection(FunctionalTestWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine("Initializing 111");
        _client = _factory.CreateClient();
        await LoginAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task LoginAsync()
    {
        const string email = "test@example.com";
        const string password = "StrongP@ssw0rd";
        const string deviceId = "3ee36e07-9c71-464c-9782-a0bab2057a77";

        var loginRequest = new
        {
            Email = email,
            Password = password,
            DeviceId = deviceId,
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            await RegisterAndLoginAsync();
        }

        response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.TokenValue.Should().NotBeNullOrEmpty();

        _authToken = loginResponse.TokenValue;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

        await FetchUserAsync();
    }

    private async Task RegisterAndLoginAsync()
    {
        const string email = "test@example.com";
        const string password = "StrongP@ssw0rd";

        var registerRequest = new
        {
            Email = email,
            Password = password,
            ConfirmPassword = password,
            FirstName = "Test",
            LastName = "User",
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task FetchUserAsync()
    {
        var response = await _client.GetAsync("/api/users/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        _user = user!;
    }

    public HttpClient GetClient() => _client;

    public string GetAuthToken() => _authToken;

    public UserDto GetUser() => _user;

    public UserDto GetUserAsync() => _user;
}
