using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class RefreshTokenTests
{
    private readonly ISender _sender;
    private readonly IAuthCookiesManager _authCookiesManager;
    private readonly RefreshToken _endpoint;

    public RefreshTokenTests()
    {
        _sender = Substitute.For<ISender>();
        _authCookiesManager = Substitute.For<IAuthCookiesManager>();
        _endpoint = new RefreshToken(_sender, _authCookiesManager);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnOkWithTokenResponse()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var refreshTokenValue = "refresh-token-value";
        var newTokenValue = "new-jwt-token";
        var userId = UserId.NewId();
        var newRefreshToken = Token.Create(
            TokenId.NewId(),
            "new-refresh-token-value",
            userId,
            TokenType.RefreshToken,
            DateTime.UtcNow.AddDays(7),
            deviceId
        ).Value;

        var request = new RefreshTokenRequest(deviceId);

        _authCookiesManager
            .GetRefreshTokenFromCookie()
            .Returns(Result.Success(refreshTokenValue));

        _sender
            .Send(Arg.Any<RefreshJwtTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<(TokenResponse, Token)>((new TokenResponse(newTokenValue), newRefreshToken))));

        _authCookiesManager
            .SetRefreshTokenCookie(Arg.Any<Token>())
            .Returns(Result.Success());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<TokenResponse>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.TokenValue.Should().Be(newTokenValue);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<RefreshJwtTokenCommand>(c =>
                    c.RefreshTokenValue == refreshTokenValue && c.DeviceId == deviceId),
                Arg.Any<CancellationToken>()
            );

        _authCookiesManager
            .Received(1)
            .SetRefreshTokenCookie(newRefreshToken);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRefreshTokenCookieNotFound_ShouldReturnUnauthorized()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var request = new RefreshTokenRequest(deviceId);

        var error = new Error("Cookie", "Refresh token not found");
        _authCookiesManager
            .GetRefreshTokenFromCookie()
            .Returns(Result.Failure<string>(error));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<UnauthorizedHttpResult>();

        await _sender
            .DidNotReceive()
            .Send(Arg.Any<RefreshJwtTokenCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var refreshTokenValue = "invalid-refresh-token";
        var request = new RefreshTokenRequest(deviceId);

        _authCookiesManager
            .GetRefreshTokenFromCookie()
            .Returns(Result.Success(refreshTokenValue));

        var error = new Error("Token", "Invalid refresh token");
        _sender
            .Send(Arg.Any<RefreshJwtTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<(TokenResponse, Token)>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenCookieSettingFails_ShouldReturnBadRequest()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var refreshTokenValue = "refresh-token-value";
        var newTokenValue = "new-jwt-token";
        var userId = UserId.NewId();
        var newRefreshToken = Token.Create(
            TokenId.NewId(),
            "new-refresh-token-value",
            userId,
            TokenType.RefreshToken,
            DateTime.UtcNow.AddDays(7),
            deviceId
        ).Value;

        var request = new RefreshTokenRequest(deviceId);

        _authCookiesManager
            .GetRefreshTokenFromCookie()
            .Returns(Result.Success(refreshTokenValue));

        _sender
            .Send(Arg.Any<RefreshJwtTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<(TokenResponse, Token)>((new TokenResponse(newTokenValue), newRefreshToken))));

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
