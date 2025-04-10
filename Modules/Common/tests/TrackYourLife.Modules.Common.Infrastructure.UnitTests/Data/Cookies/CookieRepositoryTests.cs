using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Data.Cookies;

public class CookieRepositoryTests : IDisposable
{
    private readonly CommonWriteDbContext _dbContext;
    private readonly CookieRepository _sut;
    private bool _disposed;

    public CookieRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CommonWriteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CommonWriteDbContext(options, null!);
        _sut = new CookieRepository(_dbContext);
    }

    [Fact]
    public async Task GetByNameAndDomainAsync_WhenCookieExists_ShouldReturnCookie()
    {
        // Arrange
        var cookieName = "test-cookie";
        var cookieDomain = "test-domain";
        var expectedCookie = Cookie
            .Create(CookieId.NewId(), cookieName, "value", cookieDomain, "/")
            .Value;

        _dbContext.Cookies.Add(expectedCookie);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByNameAndDomainAsync(
            cookieName,
            cookieDomain,
            CancellationToken.None
        );

        // Assert
        result.Should().BeEquivalentTo(expectedCookie);
    }

    [Fact]
    public async Task GetByNameAndDomainAsync_WhenCookieDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var cookieName = "non-existent";
        var cookieDomain = "test-domain";
        var existingCookie = Cookie
            .Create(CookieId.NewId(), "different-name", "value", cookieDomain, "/")
            .Value;

        _dbContext.Cookies.Add(existingCookie);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByNameAndDomainAsync(
            cookieName,
            cookieDomain,
            CancellationToken.None
        );

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAndDomainAsync_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cookieName = "test-cookie";
        var cookieDomain = "test-domain";
        var cancellationToken = new CancellationToken(true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.GetByNameAndDomainAsync(cookieName, cookieDomain, cancellationToken)
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }
}
