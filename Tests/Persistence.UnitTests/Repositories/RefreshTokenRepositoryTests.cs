using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Persistence.Repositories;
using TrackYourLifeDotnet.Domain.Enums;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Persistence.UnitTests.Repositories;

public class UserTokenRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserTokenRepository _sut;

    public UserTokenRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options, null);

        _sut = new UserTokenRepository(_context);
    }

    [Fact]
    public async Task GetByValueAsync_ReturnsToken_WhenTokenExists()
    {
        // Arrange
        var token = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "token1",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );
        _context.UserTokens.Add(token);
        _context.SaveChanges();

        // Act
        var tokenFromDb = await _sut.GetByValueAsync(token.Value, CancellationToken.None);

        // Assert
        Assert.NotNull(tokenFromDb);
        Assert.Equal(token, tokenFromDb);
    }

    [Fact]
    public async Task GetByValueAsync_ReturnsNull_WhenTokenDoesNotExist()
    {
        // Arrange
        const string tokenValue = "nonexistenttoken";

        // Act
        var result = await _sut.GetByValueAsync(tokenValue, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsRefreshToken_WhenRefreshTokenExists()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "value",
            userId,
            UserTokenTypes.RefreshToken
        );
        _context.UserTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act
        var refreshTokenFromDb = await _sut.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(refreshTokenFromDb);
        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNull_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());

        // Act
        var result = await _sut.GetByUserIdAsync(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void Add_AddsTokenToContext()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );

        // Act
        await _sut.AddAsync(refreshToken, CancellationToken.None);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.UserTokens.FirstOrDefault(t => t.Id == refreshToken.Id);
        Assert.Contains(refreshToken, _context.UserTokens);
        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public async void Add_Should_ThrowException_WhenRefreshTokenWithDuplicateIdIsAdded()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );
        _context.UserTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act and assert
        await _sut.AddAsync(refreshToken, CancellationToken.None);
        Assert.Throws<ArgumentException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Remove_RemovesTokenFromContext()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );
        _context.UserTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act
        _sut.Remove(refreshToken);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.UserTokens.FirstOrDefault(t => t.Id == refreshToken.Id);
        Assert.DoesNotContain(refreshToken, _context.UserTokens);
        Assert.Null(refreshTokenFromDb);
    }

    [Fact]
    public void Remove_ShouldThrowException_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );

        // Act
        _sut.Update(refreshToken);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Update_UpdatesTokenInContext()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );
        _context.UserTokens.Add(refreshToken);
        _context.SaveChanges();

        const string newTokenValue = "newTokenValue";
        refreshToken.UpdateToken(newTokenValue);

        // Act
        _sut.Update(refreshToken);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.UserTokens.FirstOrDefault(t => t.Id == refreshToken.Id);

        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public void Update_Should_ThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "tokenValue",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );

        // Act
        _sut.Update(refreshToken);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
