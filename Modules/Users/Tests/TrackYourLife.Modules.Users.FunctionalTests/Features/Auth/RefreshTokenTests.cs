using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Login;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Register;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class RefreshTokenTests : BaseIntegrationTest
{
    public RefreshTokenTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnNewToken()
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

        // Login to get initial tokens
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            DeviceId = deviceId,
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/users/auth/login", loginCommand);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var refreshResponse = await _client.PostAsync("/api/users/auth/refresh", null);

        // Assert
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.PostAsync("/api/users/auth/refresh", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
