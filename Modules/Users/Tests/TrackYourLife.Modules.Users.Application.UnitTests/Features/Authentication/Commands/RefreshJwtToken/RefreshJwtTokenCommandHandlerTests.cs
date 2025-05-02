using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.RefreshJwtToken;

public class RefreshJwtTokenCommandHandlerTests
{
    private readonly ITokenQuery _tokenQuery;
    private readonly IUserQuery _userQuery;
    private readonly IAuthService _authService;
    private readonly RefreshJwtTokenCommandHandler _handler;

    public RefreshJwtTokenCommandHandlerTests()
    {
        _tokenQuery = Substitute.For<ITokenQuery>();
        _userQuery = Substitute.For<IUserQuery>();
        _authService = Substitute.For<IAuthService>();
        _handler = new RefreshJwtTokenCommandHandler(_tokenQuery, _userQuery, _authService);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var refreshTokenValue = "valid-refresh-token";
        var command = new RefreshJwtTokenCommand(refreshTokenValue, deviceId);

        var refreshToken = TokenFaker.GenerateReadModel(
            expiresAt: DateTime.UtcNow.AddDays(1),
            deviceId: deviceId
        );

        var user = UserFaker.GenerateReadModel();

        var newJwtToken = "new-jwt-token";
        var newRefreshToken = TokenFaker.Generate(
            userId: user.Id,
            value: "new-refresh-token",
            expiresAt: DateTime.UtcNow.AddDays(1),
            deviceId: deviceId
        );

        _tokenQuery
            .GetByValueAsync(refreshTokenValue, Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        _userQuery.GetByIdAsync(refreshToken.UserId, Arg.Any<CancellationToken>()).Returns(user);

        _authService
            .RefreshUserAuthTokensAsync(user, deviceId, Arg.Any<CancellationToken>())
            .Returns(Result.Success((newJwtToken, newRefreshToken)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Item1.TokenValue.Should().Be(newJwtToken);
        result.Value.Item2.Value.Should().Be(newRefreshToken.Value);
        result.Value.Item2.Should().Be(newRefreshToken);
    }

    [Fact]
    public async Task Handle_WithInvalidRefreshToken_ReturnsFailure()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var refreshTokenValue = "invalid-refresh-token";
        var command = new RefreshJwtTokenCommand(refreshTokenValue, deviceId);

        _tokenQuery
            .GetByValueAsync(refreshTokenValue, Arg.Any<CancellationToken>())
            .Returns((TokenReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TokenErrors.RefreshToken.Invalid);
    }

    [Fact]
    public async Task Handle_WithExpiredRefreshToken_ReturnsFailure()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var refreshTokenValue = "expired-refresh-token";
        var command = new RefreshJwtTokenCommand(refreshTokenValue, deviceId);

        var refreshToken = TokenFaker.GenerateReadModel(
            value: refreshTokenValue,
            expiresAt: DateTime.UtcNow.AddDays(-1),
            deviceId: deviceId
        );

        _tokenQuery
            .GetByValueAsync(refreshTokenValue, Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TokenErrors.RefreshToken.Expired);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ReturnsFailure()
    {
        // Arrange
        var deviceId = DeviceId.NewId();
        var refreshTokenValue = "valid-refresh-token";
        var command = new RefreshJwtTokenCommand(refreshTokenValue, deviceId);

        var refreshToken = TokenFaker.GenerateReadModel(
            value: refreshTokenValue,
            expiresAt: DateTime.UtcNow.AddDays(1),
            deviceId: deviceId
        );

        _tokenQuery
            .GetByValueAsync(refreshTokenValue, Arg.Any<CancellationToken>())
            .Returns(refreshToken);

        _userQuery
            .GetByIdAsync(refreshToken.UserId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TokenErrors.RefreshToken.Invalid);
    }
}
