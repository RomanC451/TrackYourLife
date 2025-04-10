using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Login;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class LoginTests : BaseIntegrationTest
{
    public LoginTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "wrongpassword",
            DeviceId = Guid.NewGuid(),
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
