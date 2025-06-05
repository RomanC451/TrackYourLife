using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.FunctionalTests;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Users.FunctionalTests.Features;

[Collection("Users Integration Tests")]
public class AuthenticationTests(UsersFunctionalTestWebAppFactory factory)
    : UsersBaseIntegrationTest(factory)
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldReturnNoContent()
    {
        var client = CreateUnauthorizedClient();

        // Arrange
        var request = new RegisterUserRequest(
            _faker.Internet.Email(),
            $"{_faker.Internet.Password()}A1123123!",
            _faker.Person.FirstName,
            _faker.Person.LastName
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Verify user was created
        var user = await _usersWriteDbContext.Users.FirstOrDefaultAsync(u =>
            u.Email == Email.Create(request.Email).Value
        );
        user.Should().NotBeNull();
        user!.FirstName.Value.Should().Be(request.FirstName);
        user.LastName.Value.Should().Be(request.LastName);
    }

    [Fact]
    public async Task RegisterUser_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var existingUser = UserFaker.Generate();
        await _usersWriteDbContext.Users.AddAsync(existingUser);
        await _usersWriteDbContext.SaveChangesAsync();

        var request = new RegisterUserRequest(
            existingUser.Email.Value,
            $"{_faker.Internet.Password()}1!",
            _faker.Person.FirstName,
            _faker.Person.LastName
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );
        error.Should().NotBeNull();
        error!.Type.Should().Be("ValidationError");
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var registerRequest = new RegisterUserRequest(
            _faker.Internet.Email(),
            $"{_faker.Internet.Password()}dasdasdA1!",
            _faker.Person.FirstName,
            _faker.Person.LastName
        );
        var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

        await registerResponse.ShouldHaveStatusCode(HttpStatusCode.Created);

        var loginRequest = new LoginUserRequest(
            registerRequest.Email,
            registerRequest.Password,
            DeviceId.NewId()
        );

        // Act
        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        await loginResponse.ShouldHaveStatusCode(HttpStatusCode.OK);
        var tokenResponse = await loginResponse.Content.ReadFromJsonAsync<TokenResponse>();
        tokenResponse.Should().NotBeNull();
        tokenResponse!.TokenValue.Should().NotBeNullOrEmpty();

        // Verify refresh token cookie was set
        var cookies = loginResponse.Headers.GetValues("Set-Cookie");
        cookies.Should().Contain(c => c.Contains("refreshToken"));
    }

    [Fact]
    public async Task LoginUser_WithInvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var request = new
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!",
            DeviceId = DeviceId.NewId(),
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );
        error.Should().NotBeNull();
        error!.Type.Should().Be("User.InvalidCredentials");
    }

    [Fact]
    public async Task VerifyEmail_WithValidToken_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateUnauthorizedClient();

        var users = await _usersWriteDbContext.Users.ToListAsync();

        users.Should().NotBeNullOrEmpty();

        var user = UserFaker.Generate();
        await _usersWriteDbContext.Users.AddAsync(user);
        await _usersWriteDbContext.SaveChangesAsync();

        // Create email verification token
        var token = Token
            .Create(
                TokenId.NewId(),
                _faker.Random.AlphaNumeric(32),
                user.Id,
                TokenType.EmailVerificationToken,
                DateTime.UtcNow.AddHours(1)
            )
            .Value;

        await _usersWriteDbContext.Tokens.AddAsync(token);
        await _usersWriteDbContext.SaveChangesAsync();

        var request = new VerifyEmailRequest(token.Value);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/verify-email", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify user is now verified
        var updatedUser = await _usersWriteDbContext
            .Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == user.Id);
        updatedUser.Should().NotBeNull();
        updatedUser!.VerifiedOnUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task VerifyEmail_WithInvalidToken_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var request = new VerifyEmailRequest(_faker.Random.AlphaNumeric(32));

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/verify-email", request);

        // Assert
        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );
        error.Should().NotBeNull();
        error!.Type.Should().Be("EmailVerificationToken.Invalid");
    }

    [Fact]
    public async Task ResendVerificationEmail_WithValidEmail_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var user = UserFaker.Generate();
        await _usersWriteDbContext.Users.AddAsync(user);
        await _usersWriteDbContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/resend-verification-email",
            new { Email = user.Email.Value }
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ResendVerificationEmail_WithNonExistingEmail_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var request = new ResendEmailVerificationRequest(_faker.Internet.Email());

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/resend-verification-email", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LogoutUser_WithValidToken_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateUnauthorizedClient();

        var email = _faker.Internet.Email();
        var password = $"{_faker.Internet.Password()}123!";
        var firstName = _faker.Person.FirstName;
        var lastName = _faker.Person.LastName;
        var deviceId = DeviceId.NewId();
        await client.RegisterAndLoginNewUserAsync(
            email: email,
            password: password,
            firstName: firstName,
            lastName: lastName,
            deviceId: deviceId
        );

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/logout",
            new { DeviceId = deviceId, LogOutAllDevices = false }
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify refresh token cookie was deleted
        var cookies = response.Headers.GetValues("Set-Cookie");
        cookies
            .Should()
            .Contain(c => c.Contains("refreshToken") && c.Contains("expires=Thu, 01 Jan 1970"));

        // TODO: Create a user banned list and check if the user is in the list after logout, then remove the user from the list after login

        var unauthorizedUserResponse = await client.GetAsync("/api/users/me");
        await unauthorizedUserResponse.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                Email = email,
                Password = password,
                DeviceId = deviceId,
            }
        );
        await loginResponse.ShouldHaveStatusCode(HttpStatusCode.OK);

        var userResponse = await client.GetAsync("/api/users/me");
        await userResponse.ShouldHaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ShouldReturnNewToken()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        var deviceId = DeviceId.NewId();
        await client.RegisterAndLoginNewUserAsync(deviceId: deviceId);

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/auth/refresh-token",
            new { DeviceId = deviceId }
        );

        // Assert
        var tokenResponse = await response.ShouldHaveStatusCodeAndContent<TokenResponse>(
            HttpStatusCode.OK
        );
        tokenResponse.Should().NotBeNull();
        tokenResponse!.TokenValue.Should().NotBeNullOrEmpty();

        // Verify new refresh token cookie was set
        var cookies = response.Headers.GetValues("Set-Cookie");
        cookies.Should().Contain(c => c.Contains("refreshToken"));
    }

    [Fact]
    public async Task RefreshToken_WithInvalidRefreshToken_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = CreateUnauthorizedClient();
        await client.RegisterAndLoginNewUserAsync();

        var request = new { DeviceId = DeviceId.NewId() };

        await _usersWriteDbContext.SaveChangesAsync();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/refresh-token", request);

        // Assert
        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );

        error.Should().NotBeNull();
        error!.Type.Should().Be("RefreshToken.Invalid");
    }
}
