using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Services;

public class AuthServiceTests
{
    private readonly IJwtProvider _jwtProvider;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IOptions<RefreshTokenCookieOptions> _refreshTokenCookieOptions;
    private readonly IOptions<ClientAppOptions> _clientAppOptions;
    private readonly IAuthorizationBlackListStorage _authorizationBlackListStorage;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthService _authService;
    private readonly UserId _testUserId;
    private readonly DeviceId _testDeviceId;

    public AuthServiceTests()
    {
        _jwtProvider = Substitute.For<IJwtProvider>();
        _tokenRepository = Substitute.For<ITokenRepository>();
        _unitOfWork = Substitute.For<IUsersUnitOfWork>();
        _refreshTokenCookieOptions = Microsoft.Extensions.Options.Options.Create(
            new RefreshTokenCookieOptions { DaysToExpire = 7 }
        );
        _clientAppOptions = Microsoft.Extensions.Options.Options.Create(
            new ClientAppOptions
            {
                BaseUrl = "https://test.com",
                EmailVerificationPath = "verify-email",
            }
        );

        _testUserId = UserId.Create(Guid.NewGuid());
        _testDeviceId = DeviceId.Create(Guid.NewGuid());

        _authorizationBlackListStorage = Substitute.For<IAuthorizationBlackListStorage>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _authService = new AuthService(
            _jwtProvider,
            _tokenRepository,
            _unitOfWork,
            _refreshTokenCookieOptions,
            _clientAppOptions,
            _authorizationBlackListStorage,
            _httpContextAccessor
        );
    }

    [Fact]
    public async Task RefreshUserAuthTokensAsync_NewToken_ShouldCreateNewToken()
    {
        // Arrange
        var user = UserFaker.GenerateReadModel(id: _testUserId);
        var jwtToken = "test-jwt-token";
        var expiresAt = DateTime.UtcNow.AddDays(7);

        _jwtProvider.Generate(user).Returns(jwtToken);
        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token>());

        // Act
        var result = await _authService.RefreshUserAuthTokensAsync(
            user,
            _testDeviceId,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().Be(jwtToken);
        result.Value.Item2.Value.Should().NotBeEmpty();
        result.Value.Item2.ExpiresAt.Should().BeCloseTo(expiresAt, TimeSpan.FromSeconds(1));
        result.Value.Item2.UserId.Should().Be(_testUserId);
        result.Value.Item2.DeviceId.Should().Be(_testDeviceId);

        await _tokenRepository.Received(1).AddAsync(Arg.Any<Token>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshUserAuthTokensAsync_ExistingToken_ShouldUpdateToken()
    {
        // Arrange
        Token updatedToken = null!;

        var user = UserFaker.GenerateReadModel(id: _testUserId);
        var jwtToken = "test-jwt-token";
        var existingToken = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.RefreshToken,
            deviceId: _testDeviceId
        );

        var old_value = existingToken.Value;

        _jwtProvider.Generate(user).Returns(jwtToken);
        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { existingToken });

        _tokenRepository
            .When(x => x.Update(Arg.Any<Token>()))
            .Do(x => updatedToken = x.Arg<Token>());

        // Act
        var result = await _authService.RefreshUserAuthTokensAsync(
            user,
            _testDeviceId,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Item1.Should().Be(jwtToken);
        result.Value.Item2.Value.Should().NotBe(old_value);
        result
            .Value.Item2.ExpiresAt.Should()
            .BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));

        result.Value.Item2.UserId.Should().Be(_testUserId);
        result.Value.Item2.DeviceId.Should().Be(_testDeviceId);

        updatedToken.Should().NotBeNull();
        updatedToken.Value.Should().NotBe(old_value);
        updatedToken
            .ExpiresAt.Should()
            .BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
        updatedToken.UserId.Should().Be(_testUserId);
        updatedToken.DeviceId.Should().Be(_testDeviceId);

        await _tokenRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<Token>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LogOutUserAsync_LogOutAllDevices_ShouldRemoveAllTokens()
    {
        // Arrange
        var tokens = new List<Token>
        {
            TokenFaker.Generate(
                userId: _testUserId,
                type: TokenType.RefreshToken,
                deviceId: DeviceId.Create(Guid.NewGuid())
            ),
            TokenFaker.Generate(
                userId: _testUserId,
                type: TokenType.RefreshToken,
                deviceId: DeviceId.Create(Guid.NewGuid())
            ),
        };

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(tokens);

        // Act
        await _authService.LogOutUserAsync(
            _testUserId,
            _testDeviceId,
            true,
            CancellationToken.None
        );

        // Assert
        _tokenRepository.Received(2).Remove(Arg.Any<Token>());
        _authorizationBlackListStorage.DidNotReceive().Add(Arg.Any<LoggedOutUser>());
    }

    [Fact]
    public async Task LogOutUserAsync_SingleDevice_ShouldRemoveOnlyDeviceToken()
    {
        // Arrange
        var deviceToken = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.RefreshToken,
            deviceId: _testDeviceId
        );

        var otherToken = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.RefreshToken,
            deviceId: DeviceId.Create(Guid.NewGuid())
        );

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { deviceToken, otherToken });

        // Act
        await _authService.LogOutUserAsync(
            _testUserId,
            _testDeviceId,
            false,
            CancellationToken.None
        );

        // Assert
        _tokenRepository.Received(1).Remove(deviceToken);
        _tokenRepository.DidNotReceive().Remove(otherToken);
        _authorizationBlackListStorage.DidNotReceive().Add(Arg.Any<LoggedOutUser>());
    }

    [Fact]
    public async Task LogOutUserAsync_WithJwtToken_ShouldAddToBlackList()
    {
        // Arrange
        var deviceToken = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.RefreshToken,
            deviceId: _testDeviceId
        );

        var jwtToken = "test-jwt-token";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers.Authorization = $"Bearer {jwtToken}";
        _httpContextAccessor.HttpContext.Returns(httpContext);

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { deviceToken });

        // Act
        await _authService.LogOutUserAsync(
            _testUserId,
            _testDeviceId,
            false,
            CancellationToken.None
        );

        // Assert
        _tokenRepository.Received(1).Remove(deviceToken);
        _authorizationBlackListStorage
            .Received(1)
            .Add(
                Arg.Is<LoggedOutUser>(u =>
                    u.Token == jwtToken && u.UserId == _testUserId && u.DeviceId == _testDeviceId
                )
            );
    }

    [Fact]
    public async Task LogOutUserAsync_WithoutJwtToken_ShouldNotAddToBlackList()
    {
        // Arrange
        var deviceToken = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.RefreshToken,
            deviceId: _testDeviceId
        );

        var httpContext = new DefaultHttpContext();
        _httpContextAccessor.HttpContext.Returns(httpContext);

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.RefreshToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { deviceToken });

        // Act
        await _authService.LogOutUserAsync(
            _testUserId,
            _testDeviceId,
            false,
            CancellationToken.None
        );

        // Assert
        _tokenRepository.Received(1).Remove(deviceToken);
        _authorizationBlackListStorage.DidNotReceive().Add(Arg.Any<LoggedOutUser>());
    }

    [Fact]
    public async Task GenerateEmailVerificationLinkAsync_ShouldReturnValidLink()
    {
        // Arrange
        var token = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.EmailVerificationToken
        );

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.EmailVerificationToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { token });

        // Act
        var result = await _authService.GenerateEmailVerificationLinkAsync(
            _testUserId,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be($"https://test.com/verify-email?token={token.Value}");
    }

    [Fact]
    public async Task GenerateEmailVerificationLinkAsync_ExpiredToken_ShouldGenerateNewToken()
    {
        // Arrange
        var token = TokenFaker.Generate(
            userId: _testUserId,
            type: TokenType.EmailVerificationToken
        );

        var expiresAtProperty = typeof(Token).GetProperty(
            "ExpiresAt",
            BindingFlags.Instance | BindingFlags.Public
        );
        expiresAtProperty!.SetValue(token, DateTime.UtcNow.AddMinutes(-1));

        _tokenRepository
            .GetByUserIdAndTypeAsync(
                _testUserId,
                TokenType.EmailVerificationToken,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<Token> { token });

        // Act
        var result = await _authService.GenerateEmailVerificationLinkAsync(
            _testUserId,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe($"https://test.com/verify-email?token={token.Value}");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
