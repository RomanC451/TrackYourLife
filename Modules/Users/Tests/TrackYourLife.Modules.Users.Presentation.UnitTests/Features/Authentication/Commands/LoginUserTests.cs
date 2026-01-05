using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class LoginUserTests
{
    private readonly ISender _sender;
    private readonly IAuthCookiesManager _authCookiesManager;
    private readonly LoginUser _endpoint;

    public LoginUserTests()
    {
        _sender = Substitute.For<ISender>();
        _authCookiesManager = Substitute.For<IAuthCookiesManager>();
        _endpoint = new LoginUser(_sender, _authCookiesManager);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnOkWithTokenResponse()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var deviceId = new DeviceId(Guid.NewGuid());
        var tokenValue = "jwt-token";
        var userId = UserId.NewId();
        var refreshToken = Token.Create(
            TokenId.NewId(),
            "refresh-token-value",
            userId,
            TokenType.RefreshToken,
            DateTime.UtcNow.AddDays(7),
            deviceId
        ).Value;

        var request = new LoginUserRequest(email, password, deviceId);

        _sender
            .Send(Arg.Any<LogInUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<(string, Token)>((tokenValue, refreshToken))));

        _authCookiesManager
            .SetRefreshTokenCookie(Arg.Any<Token>())
            .Returns(Result.Success());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<TokenResponse>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.TokenValue.Should().Be(tokenValue);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<LogInUserCommand>(c =>
                    c.Email == email && c.Password == password && c.DeviceId == deviceId),
                Arg.Any<CancellationToken>()
            );

        _authCookiesManager
            .Received(1)
            .SetRefreshTokenCookie(refreshToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword";
        var deviceId = new DeviceId(Guid.NewGuid());
        var request = new LoginUserRequest(email, password, deviceId);

        var error = new Error("Authentication", "Invalid credentials");
        _sender
            .Send(Arg.Any<LogInUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<(string, Token)>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();

        _authCookiesManager
            .DidNotReceive()
            .SetRefreshTokenCookie(Arg.Any<Token>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCookieSettingFails_ShouldReturnBadRequest()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var deviceId = new DeviceId(Guid.NewGuid());
        var tokenValue = "jwt-token";
        var userId = UserId.NewId();
        var refreshToken = Token.Create(
            TokenId.NewId(),
            "refresh-token-value",
            userId,
            TokenType.RefreshToken,
            DateTime.UtcNow.AddDays(7),
            deviceId
        ).Value;

        var request = new LoginUserRequest(email, password, deviceId);

        _sender
            .Send(Arg.Any<LogInUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<(string, Token)>((tokenValue, refreshToken))));

        var cookieError = new Error("Cookie", "Failed to set cookie");
        _authCookiesManager
            .SetRefreshTokenCookie(Arg.Any<Token>())
            .Returns(Result.Failure(cookieError));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
