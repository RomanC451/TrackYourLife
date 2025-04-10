using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Login;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Register;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class LogoutTests : BaseIntegrationTest
{
    public LogoutTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var email = "test@example.com";
        var password = "StrongP@ssw0rd";

        // Register a new user
        var registerCommand = new RegisterCommand
        {
            Email = email,
            Password = password,
            FirstName = "John",
            LastName = "Doe",
        };
        await _client.PostAsJsonAsync("/api/users/auth/register", registerCommand);

        // Login to get tokens
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            DeviceId = deviceId,
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/users/auth/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var logoutResponse = await _client.PostAsync("/api/users/auth/logout", null);

        // Assert
        logoutResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify that refresh token is no longer valid
        var refreshResponse = await _client.PostAsync("/api/users/auth/refresh", null);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Logout_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/users/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
