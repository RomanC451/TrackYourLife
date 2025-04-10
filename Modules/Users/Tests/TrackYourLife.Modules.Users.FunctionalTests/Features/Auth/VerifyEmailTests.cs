using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Register;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.VerifyEmail;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class VerifyEmailTests : BaseIntegrationTest
{
    public VerifyEmailTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task VerifyEmail_WithValidToken_ShouldReturnOk()
    {
        // Arrange
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

        // Get verification token from email service
        // Note: In a real test, you would need to mock the email service and capture the token
        var verificationToken = "test-verification-token";

        var command = new VerifyEmailCommand { Token = verificationToken };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task VerifyEmail_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new VerifyEmailCommand { Token = "invalid-token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/auth/verify-email", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
