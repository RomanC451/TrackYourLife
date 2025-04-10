using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TrackYourLife.Modules.Users.Application.Features.Auth.Commands.Register;
using TrackYourLife.Modules.Users.FunctionalTests.TestServer;
using TrackYourLife.Modules.Users.FunctionalTests.Utils;
using Xunit;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features.Auth;

public class RegisterTests : BaseIntegrationTest
{
    public RegisterTests(IntegrationTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "newuser@example.com",
            Password = "StrongP@ssw0rd",
            FirstName = "John",
            LastName = "Doe",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = "invalid-email",
            Password = "StrongP@ssw0rd",
            FirstName = "John",
            LastName = "Doe",
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users/auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
