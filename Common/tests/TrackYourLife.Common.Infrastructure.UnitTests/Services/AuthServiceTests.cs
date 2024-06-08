using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using TrackYourLife.Application.Core.Abstractions.Authentication;
using TrackYourLife.Domain.Errors;
using TrackYourLife.Domain.Repositories;
using TrackYourLife.Domain.Shared;
using TrackYourLife.Infrastructure.Services;
using TrackYourLife.Domain.Users.Repositories;
using TrackYourLife.Domain.Users;
using TrackYourLife.Domain.Users.ValueObjects;
using TrackYourLife.Domain.Users.StrongTypes;

namespace TrackYourLife.Common.Infrastructure.UnitTests.Services;

public class AuthServiceTests
{
    private readonly AuthService _sut;
    private readonly Mock<IUserTokenRepository> _userTokenRepository = new();
    private readonly Mock<IJwtProvider> _jwtProvider = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new();

    private readonly Mock<HttpContext> _httpContext = new();

    public AuthServiceTests()
    {
        _httpContextAccessor.Setup(x => x.HttpContext).Returns(_httpContext.Object);

        _sut = new AuthService(
            _jwtProvider.Object,
            _userTokenRepository.Object,
            _unitOfWork.Object,
            _httpContextAccessor.Object
        );
    }

    [Fact]
    public async void RefreshUserAuthTokens_ReturnsNewJwtTokenAndUpdateTheOldRefreshTokenFromDb_WhenTheRefreshTokenOfUserExists()
    {
        //Arrange
        User user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("example@email.com").Value,
            new HashedPassword("password"),
            Name.Create("first").Value,
            Name.Create("last").Value
        );

        const string newJwtTokenString = "newJwtToken";
        const string newRefreshTokenString = "newRefreshToken";
        const string oldRefreshTokenString = "oldRefreshToken";

        var refreshTokenFromDb = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            oldRefreshTokenString,
            user.Id,
            UserTokenTypes.RefreshToken
        );

        _jwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns(newJwtTokenString);

        _userTokenRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<UserId>(), CancellationToken.None))
            .ReturnsAsync(refreshTokenFromDb);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        //Act
        (string jwtTokenResponse, UserToken refreshTokenResponse) =
            await _sut.RefreshUserAuthTokensAsync(user, CancellationToken.None);

        //Assert
        Assert.Equal(newJwtTokenString, jwtTokenResponse);
        Assert.Equal(newRefreshTokenString, refreshTokenResponse.Value);
        Assert.NotEqual(oldRefreshTokenString, refreshTokenResponse.Value);
        Assert.Equal(refreshTokenFromDb.Value, refreshTokenResponse.Value);
        Assert.Equal(refreshTokenFromDb.Id, refreshTokenResponse.Id);
        Assert.Equal(user.Id, refreshTokenResponse.UserId);
        Assert.True(refreshTokenResponse.ExpiresAt > DateTime.UtcNow);

        _jwtProvider.Verify(x => x.Generate(user), Times.Once);
        _userTokenRepository.Verify(
            x => x.GetByUserIdAsync(user.Id, CancellationToken.None),
            Times.Once
        );
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void RefreshUserAuthTokens_ReturnsNewJwtTokenAndCreatesNewRefreshTokenAndAddItToDb_WhenTheRefreshTokenOfUserExists()
    {
        //Arrange
        User user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("example@email.com").Value,
            new HashedPassword("password"),
            Name.Create("first").Value,
            Name.Create("last").Value
        );

        const string newJwtTokenString = "newJwtToken";
        const string newRefreshTokenString = "newRefreshToken";

        _jwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns(newJwtTokenString);

        _userTokenRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<UserId>(), CancellationToken.None))
            .ReturnsAsync((UserToken)null!);

        _unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));

        //Act
        (string jwtTokenResponse, UserToken refreshTokenResponse) =
            await _sut.RefreshUserAuthTokensAsync(user, CancellationToken.None);

        //Assert
        Assert.Equal(newJwtTokenString, jwtTokenResponse);
        Assert.Equal(newRefreshTokenString, refreshTokenResponse.Value);
        Assert.IsType<Guid>(refreshTokenResponse.Id);
        Assert.Equal(user.Id, refreshTokenResponse.UserId);
        Assert.True(refreshTokenResponse.ExpiresAt > DateTime.UtcNow);

        _jwtProvider.Verify(x => x.Generate(user), Times.Once);
        _userTokenRepository.Verify(
            x => x.GetByUserIdAsync(user.Id, CancellationToken.None),
            Times.Once
        );
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void GetUserIdFromJwtToken_ReturnsUserId_WhenJwtTokenIsValid()
    {
        //Arrange
        const string jwtToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwOWI0NjljZS0yNzg0LTQyYTctOTc2NC1jOThhNGIxMDNhNWYiLCJlbWFpbCI6ImNhdGFsaW4ucm9tYW40NTFAZ21haWwuY29tIiwiZXhwIjoxNjc4OTYxODUyLCJpc3MiOiJUcmFja1lPdXJMaWZlRG90bmV0IiwiYXVkIjoiVHJhY2tZT3VyTGlmZURvdG5ldCJ9.sTuf7f4OIrKFychzTnGdBWlHUtpjXrf_B8ajuuCUr2k";
        ;
        var headers = new HeaderDictionary
        {
            ["Authorization"] = new StringValues($"Bearer {jwtToken}")
        };

        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.Headers).Returns(headers);
        _httpContext.SetupGet(x => x.Request).Returns(request.Object);

        UserId userId = new(Guid.Parse("09b469ce-2784-42a7-9764-c98a4b103a5f"));

        //Act
        Result<UserId> result = _sut.GetUserIdFromJwtToken();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value);
    }

    [Theory]
    [InlineData(
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIwOWI0NjljZS0yNzg0LTQyY6ImNhdGFsaW4ucm9ttIiwiZXhwIjoxNjc4OTYxODUyLCJpc3MiOiJUcmFja1lPdXJMaWZlRG90bmV0IiwiYXVkIjoiVHJhY2tZT3VyTGlmZURvdG5ldCJ9.sTuf7f4OIrKFychzTnGdBWlHUtpjXrf_B8ajuuCUr2k"
    )]
    [InlineData("")]
    [InlineData(null)]
    public void GetUserIdFromJwtToken_ReturnsInvalidJwtTokenError_WhenJwtTokenIsInvalid(
        string jwtToken
    )
    {
        //Arrange
        var headers = new HeaderDictionary
        {
            ["Authorization"] = new StringValues($"Bearer {jwtToken}")
        };

        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.Headers).Returns(headers);
        _httpContext.SetupGet(x => x.Request).Returns(request.Object);

        //Act
        Result<UserId> result = _sut.GetUserIdFromJwtToken();

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal(result.Error.Message, DomainErrors.JwtToken.Invalid.Message);
    }

    [Fact]
    public void SetRefreshTokenCookie_SetsTheCookieAndReturnsTrue_WhenRefreshTokenIsValidAndHttpContextExists()
    {
        //Arrange
        string cookieName = string.Empty;
        string cookieValue = string.Empty;
        CookieOptions cookieOptions = new();

        var response = new Mock<HttpResponse>();
        response
            .Setup(
                x =>
                    x.Cookies.Append(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CookieOptions>()
                    )
            )
            .Callback<string, string, CookieOptions>(
                (name, value, options) =>
                {
                    cookieName = name;
                    cookieValue = value;
                    cookieOptions = options;
                }
            );
        _httpContext.SetupGet(x => x.Response).Returns(response.Object);

        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "refreshToken",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );

        //Act
        var result = _sut.SetRefreshTokenCookie(refreshToken);

        //Assert
        Assert.Equal("refreshToken", cookieName);
        Assert.Equal(refreshToken.Value, cookieValue);
        Assert.Equal(refreshToken.ExpiresAt, cookieOptions.Expires);
        Assert.True(cookieOptions.HttpOnly);
        Assert.False(cookieOptions.Secure);
        Assert.True(result.IsSuccess);

        response.Verify(
            x =>
                x.Cookies.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()),
            Times.Once
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void SetRefreshTokenCookie_ReturnsFailure_WhenRefreshTokenValueIsEmptyOrNull(
        string refreshTokenValue
    )
    {
        //Arrange
        string cookieName = string.Empty;
        string cookieValue = string.Empty;
        CookieOptions cookieOptions = new();

        var response = new Mock<HttpResponse>();
        response
            .Setup(
                x =>
                    x.Cookies.Append(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<CookieOptions>()
                    )
            )
            .Callback<string, string, CookieOptions>(
                (name, value, options) =>
                {
                    cookieName = name;
                    cookieValue = value;
                    cookieOptions = options;
                }
            );
        _httpContext.SetupGet(x => x.Response).Returns(response.Object);

        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            refreshTokenValue,
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );

        //Act
        var result = _sut.SetRefreshTokenCookie(refreshToken);

        //Assert
        Assert.Empty(cookieName);
        Assert.Empty(cookieValue);
        Assert.Null(cookieOptions.Expires);
        Assert.False(cookieOptions.HttpOnly);
        Assert.False(cookieOptions.Secure);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void GetHttpContextJwtToken_ReturnsJwtToken_WhenAuthHeaderExistsAndContainJwtToken()
    {
        //Arrange
        const string jwtToken = "valid_jwt_token";
        var headers = new HeaderDictionary
        {
            ["Authorization"] = new StringValues($"Bearer {jwtToken}")
        };

        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.Headers).Returns(headers);
        _httpContext.SetupGet(x => x.Request).Returns(request.Object);

        //Act
        var result = _sut.GetHttpContextJwtToken();
        //Assert

        Assert.True(result.IsSuccess);
        Assert.Equal(jwtToken, result.Value);
    }

    [Fact]
    public void GetHttpContextJwtToken_ReturnsFailureResponse_WhenAuthHeaderDontExists()
    {
        //Arrange
        var headers = new HeaderDictionary();

        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.Headers).Returns(headers);
        _httpContext.SetupGet(x => x.Request).Returns(request.Object);

        //Act
        var result = _sut.GetHttpContextJwtToken();
        //Assert

        Assert.True(result.IsFailure);
        Assert.Equal(InfrastructureErrors.HttpContext.NotExists, result.Error);
    }

    [Fact]
    public void GetHttpContextJwtToken_ReturnsFailureResult_WhenAuthHeaderIsInvalid()
    {
        //Arrange
        const string jwtToken = "valid_jwt_token";
        var headers = new HeaderDictionary { ["Authorization"] = new StringValues(jwtToken) };

        var request = new Mock<HttpRequest>();
        request.SetupGet(x => x.Headers).Returns(headers);
        _httpContext.SetupGet(x => x.Request).Returns(request.Object);

        //Act
        var result = _sut.GetHttpContextJwtToken();
        //Assert

        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.JwtToken.Invalid, result.Error);
    }

    [Fact]
    public void GetRefreshTokenFromCookie_ReturnsRefreshToken_WhenCookieExists()
    {
        // Arrange
        _httpContext.SetupGet(x => x.Request.Cookies["refreshToken"]).Returns("refreshTokenValue");

        // Act
        var result = _sut.GetRefreshTokenFromCookie();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("refreshTokenValue", result.Value);
    }

    [Fact]
    public void GetRefreshTokenFromCookie_ReturnsInvalidRefreshToken_WhenCookieDoesNotExist()
    {
        // Arrange
        _httpContext.SetupGet(x => x.Request.Cookies["refreshToken"]).Returns("");

        // Act
        var result = _sut.GetRefreshTokenFromCookie();

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(DomainErrors.RefreshToken.Invalid, result.Error);
    }
}
