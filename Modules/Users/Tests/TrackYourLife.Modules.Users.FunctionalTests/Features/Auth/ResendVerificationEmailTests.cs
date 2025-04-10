using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Register;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.ResendVerificationEmail;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class ResendVerificationEmailTests : BaseIntegrationTest
{
    public ResendVerificationEmailTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task ResendVerificationEmail_WithValidEmail_ShouldReturnOk()
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

        var command = new ResendVerificationEmailCommand { Email = email };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/auth/resend-verification-email",
            command
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResendVerificationEmail_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new ResendVerificationEmailCommand { Email = "nonexistent@example.com" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/users/auth/resend-verification-email",
            command
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
