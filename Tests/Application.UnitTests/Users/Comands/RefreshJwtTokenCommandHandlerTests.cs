using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Enums;

namespace TrackYourLifeDotnet.Application.UnitTests.Users.Comands;

public class RefreshJwtTokenCommandHandlerTests
{
    private readonly Mock<IJwtProvider> _jwtProviderMock = new();
    private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IAuthService> _authServiceMock = new();
    private readonly RefreshJwtTokenCommandHandler _sut;

    private const string refreshTokenValue = "refreshToken";

    public RefreshJwtTokenCommandHandlerTests()
    {
        _jwtProviderMock = new Mock<IJwtProvider>();
        _userTokenRepositoryMock = new Mock<IUserTokenRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authServiceMock = new Mock<IAuthService>();
        _sut = new RefreshJwtTokenCommandHandler(
            _userTokenRepositoryMock.Object,
            _userRepositoryMock.Object,
            _authServiceMock.Object
        );
    }

    private static UserToken GenerateValidRefreshToken()
    {
        return new UserToken(
            Guid.NewGuid(),
            refreshTokenValue,
            Guid.NewGuid(),
            UserTokenTypes.RefreshToken
        );
    }

    [Fact]
    public async Task Handler_WithValidRefreshToken_ReturnsNewJwtTokenAndRefreshToken()
    {
        // Arrange
        var command = new RefreshJwtTokenCommand(refreshTokenValue);
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("johndoe@example.com").Value,
            new HashedPassword("password"),
            Name.Create("John").Value,
            Name.Create("Doe").Value
        );
        var refreshToken = GenerateValidRefreshToken();

        UserToken cookieSetRefreshToken = null!;
        _authServiceMock
            .Setup(serv => serv.SetRefreshTokenCookie(It.IsAny<UserToken>()))
            .Callback<UserToken>((token) => cookieSetRefreshToken = token);
        _userTokenRepositoryMock
            .Setup(repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(refreshToken.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _jwtProviderMock.Setup(provider => provider.Generate(user)).Returns("newJwtToken");

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var response = result.Value;
        Assert.Equal("newJwtToken", response.NewJwtTokenString);
        Assert.Equal("newRefreshToken", response.NewRefreshToken.Value);
        Assert.Equal(cookieSetRefreshToken!.Value, response.NewRefreshToken.Value);

        _userTokenRepositoryMock.Verify(
            repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(refreshToken.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );
        _jwtProviderMock.Verify(provider => provider.Generate(user), Times.Once);
    }

    [Fact]
    public async Task Handler_WithInvalidRefreshToken_ReturnsFailureResultWithInvalidError()
    {
        // Arrange
        var command = new RefreshJwtTokenCommand(refreshTokenValue);

        _userTokenRepositoryMock
            .Setup(x => x.GetByValueAsync(command.RefreshToken!, CancellationToken.None))
            .ReturnsAsync((UserToken)null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.RefreshToken.Invalid, result.Error);

        _userTokenRepositoryMock.Verify(
            x => x.GetByValueAsync(command.RefreshToken!, CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task Handler_WithExpiredRefreshToken_ReturnsFailureResultWithExpiredError()
    {
        // Arrange

        var command = new RefreshJwtTokenCommand(refreshTokenValue);
        var expiredToken = GenerateValidRefreshToken();
        expiredToken!
            .GetType()!
            .GetProperty("ExpiresAt")!
            .SetValue(expiredToken, DateTime.UtcNow.AddDays(-1), null);

        _userTokenRepositoryMock
            .Setup(repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.RefreshToken.Expired, result.Error);

        _userTokenRepositoryMock.Verify(
            repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handler_WithUnkownUserIdInRefreshToken_ReturnsFailureResultWithInvalidError()
    {
        // Arrange
        var command = new RefreshJwtTokenCommand(refreshTokenValue);
        var refreshToken = GenerateValidRefreshToken();

        _userTokenRepositoryMock
            .Setup(repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        _userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(refreshToken.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.RefreshToken.Invalid, result.Error);

        _userTokenRepositoryMock.Verify(
            repo => repo.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        _userRepositoryMock.Verify(
            repo => repo.GetByIdAsync(refreshToken.UserId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
