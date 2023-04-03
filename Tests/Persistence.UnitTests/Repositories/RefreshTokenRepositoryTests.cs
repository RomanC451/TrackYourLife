using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Persistence.Repositories;
using Xunit;

namespace TrackYourLifeDotnet.Persistence.UnitTests.Repositories;

public class RefreshTokenRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options, null);

        _refreshTokenRepository = new RefreshTokenRepository(_context);
    }

    [Fact]
    public async Task GetByValueAsync_ReturnsToken_WhenTokenExists()
    {
        // Arrange
        var token = new RefreshToken(Guid.NewGuid(), "token1", Guid.NewGuid());
        _context.RefreshTokens.Add(token);
        _context.SaveChanges();

        // Act
        var tokenFromDb = await _refreshTokenRepository.GetByValueAsync(
            token.Value,
            CancellationToken.None
        );

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
        var result = await _refreshTokenRepository.GetByValueAsync(
            tokenValue,
            CancellationToken.None
        );

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsRefreshToken_WhenRefreshTokenExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = new RefreshToken(Guid.NewGuid(), "value", userId);
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act
        var refreshTokenFromDb = await _refreshTokenRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.NotNull(refreshTokenFromDb);
        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNull_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _refreshTokenRepository.GetByUserIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Add_AddsTokenToContext()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());

        // Act
        _refreshTokenRepository.Add(refreshToken);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.RefreshTokens.FirstOrDefault(
            t => t.Id == refreshToken.Id
        );
        Assert.Contains(refreshToken, _context.RefreshTokens);
        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public void Add_Should_ThrowException_WhenRefreshTokenWithDuplicateIdIsAdded()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act and assert
        _refreshTokenRepository.Add(refreshToken);
        Assert.Throws<ArgumentException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Remove_RemovesTokenFromContext()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        // Act
        _refreshTokenRepository.Remove(refreshToken);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.RefreshTokens.FirstOrDefault(
            t => t.Id == refreshToken.Id
        );
        Assert.DoesNotContain(refreshToken, _context.RefreshTokens);
        Assert.Null(refreshTokenFromDb);
    }

    [Fact]
    public void Remove_ShouldThrowException_WhenRefreshTokenDoesNotExist()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());

        // Act
        _refreshTokenRepository.Update(refreshToken);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Update_UpdatesTokenInContext()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        const string newTokenValue = "newTokenValue";
        refreshToken.UpdateToken(newTokenValue);

        // Act
        _refreshTokenRepository.Update(refreshToken);
        _context.SaveChanges();

        // Assert
        var refreshTokenFromDb = _context.RefreshTokens.FirstOrDefault(
            t => t.Id == refreshToken.Id
        );

        Assert.Equal(refreshToken, refreshTokenFromDb);
    }

    [Fact]
    public void Update_Should_ThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var refreshToken = new RefreshToken(Guid.NewGuid(), "tokenValue", Guid.NewGuid());

        // Act
        _refreshTokenRepository.Update(refreshToken);
        Assert.Throws<DbUpdateConcurrencyException>(() => _context.SaveChanges());
    }

    [Fact]
    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }
}
