using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.SharedLib.FunctionalTests;

public abstract class FunctionalTestCollection : IAsyncLifetime
{
    private readonly FunctionalTestWebAppFactory _factory;
    private HttpClient _client = null!;
    private string _authToken = null!;
    private UserDto _user = null!;

    private readonly string _deviceId = "3ee36e07-9c71-464c-9782-a0bab2057a77";

    private readonly string _email = "test@example.com";
    private readonly string _userPassword = "StrongP@ssw0rd";

    protected FunctionalTestCollection(FunctionalTestWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        await LoginAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    private async Task LoginAsync()
    {
        var loginRequest = new
        {
            Email = _email,
            Password = _userPassword,
            DeviceId = _deviceId,
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            await RegisterAndLoginAsync();
        }

        response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var str = await response.Content.ReadAsStringAsync();
        Console.WriteLine(str);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
        loginResponse.Should().NotBeNull();
        loginResponse!.TokenValue.Should().NotBeNullOrEmpty();

        _authToken = loginResponse.TokenValue;
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

        await FetchUserAsync();
        await FetchUserAsync();
    }

    private async Task RegisterAndLoginAsync()
    {
        var registerRequest = new
        {
            Email = _email,
            Password = _userPassword,
            ConfirmPassword = _userPassword,
            FirstName = "Test",
            LastName = "User",
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    private async Task FetchUserAsync()
    {
        var response = await _client.GetAsync("/api/users/me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<UserDto>(JsonOptions);
        _user = user!;
    }

    public HttpClient GetClient() => _client;

    public HttpClient CreateUnauthorizedClient() => _factory.CreateClient();

    public string GetAuthToken() => _authToken;

    public UserDto GetUser() => _user;

    public UserDto GetUserAsync() => _user;

    public string GetDeviceId() => _deviceId;

    public string GetUserEmail() => _email;

    public string GetUserPassword() => _userPassword;
}
