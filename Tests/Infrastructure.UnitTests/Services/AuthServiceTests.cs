using Microsoft.AspNetCore.Http;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Infrastructure.Services;
using Xunit;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Services;

public class AuthServiceTests
{
    private readonly AuthService _sut;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly Mock<IRefreshTokenProvider> _refreshTokenProvider = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    public AuthServiceTests()
    {
        _sut = new AuthService(
            _refreshTokenProvider.Object,
            _jwtProvider.Object,
            _refreshTokenRepository.Object,
            _unitOfWork.Object,
            _httpContextAccessor.Object
        );
    }

    [Fact]
    public async void RefreshUserAuthTokens_ReturnsNewJwtTokenAndUpdateTheOldRefreshTokenFromDb_WhenTheRefreshTokenOfUserExists()
    {
        //Arrange
        User user = User.Create(
            Guid.NewGuid(),
            Email.Create("example@email.com").Value,
            PasswordHash.Create("password").Value,
            FirstName.Create("first").Value,
            LastName.Create("last").Value
        );

        const string newJwtTokenString = "newJwtToken";
        const string newRefreshTokenString = "newRefreshToken";
        const string oldRefreshTokenString = "oldRefreshToken";

        var refreshTokenFromDb = new RefreshToken(Guid.NewGuid(), oldRefreshTokenString, user.Id);

        _jwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns(newJwtTokenString);
        _refreshTokenProvider.Setup(x => x.Generate()).Returns(newRefreshTokenString);

        _refreshTokenRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(refreshTokenFromDb);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        //Act
        (string jwtTokenResponse, RefreshToken refreshTokenResponse) =
            await _sut.RefreshUserAuthTokens(user, CancellationToken.None);

        //Assert
        Assert.Equal(newJwtTokenString, jwtTokenResponse);
        Assert.Equal(newRefreshTokenString, refreshTokenResponse.Value);
        Assert.NotEqual(oldRefreshTokenString, refreshTokenResponse.Value);
        Assert.Equal(refreshTokenFromDb.Value, refreshTokenResponse.Value);
        Assert.Equal(refreshTokenFromDb.Id, refreshTokenResponse.Id);
        Assert.Equal(user.Id, refreshTokenResponse.UserId);
        Assert.True(refreshTokenResponse.ExpiresAt > DateTime.UtcNow);

        _refreshTokenProvider.Verify(x => x.Generate(), Times.Once);
        _jwtProvider.Verify(x => x.Generate(user), Times.Once);
        _refreshTokenRepository.Verify(x => x.GetByUserIdAsync(user.Id), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void RefreshUserAuthTokens_ReturnsNewJwtTokenAndCreatesNewRefreshTokenAndAddItToDb_WhenTheRefreshTokenOfUserExists()
    {
        //Arrange
        User user = User.Create(
            Guid.NewGuid(),
            Email.Create("example@email.com").Value,
            PasswordHash.Create("password").Value,
            FirstName.Create("first").Value,
            LastName.Create("last").Value
        );

        const string newJwtTokenString = "newJwtToken";
        const string newRefreshTokenString = "newRefreshToken";

        _jwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns(newJwtTokenString);
        _refreshTokenProvider.Setup(x => x.Generate()).Returns(newRefreshTokenString);

        _refreshTokenRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((RefreshToken)null!);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        //Act
        (string jwtTokenResponse, RefreshToken refreshTokenResponse) =
            await _sut.RefreshUserAuthTokens(user, CancellationToken.None);

        //Assert
        Assert.Equal(newJwtTokenString, jwtTokenResponse);
        Assert.Equal(newRefreshTokenString, refreshTokenResponse.Value);
        Assert.IsType<Guid>(refreshTokenResponse.Id);
        Assert.Equal(user.Id, refreshTokenResponse.UserId);
        Assert.True(refreshTokenResponse.ExpiresAt > DateTime.UtcNow);

        _refreshTokenProvider.Verify(x => x.Generate(), Times.Once);
        _jwtProvider.Verify(x => x.Generate(user), Times.Once);
        _refreshTokenRepository.Verify(x => x.GetByUserIdAsync(user.Id), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
