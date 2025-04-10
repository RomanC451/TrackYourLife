using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Data.Cookies;

public class CookieQueryTests : IDisposable
{
    private readonly CommonReadDbContext _dbContext;
    private readonly CookieQuery _sut;
    private bool _disposed;

    public CookieQueryTests()
    {
        var options = new DbContextOptionsBuilder<CommonReadDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CommonReadDbContext(options, null!);
        _sut = new CookieQuery(_dbContext);
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenCookiesExist_ShouldReturnAllCookiesForDomain()
    {
        // Arrange
        var domain = "test-domain";
        var cookies = new[]
        {
            new CookieReadModel(CookieId.NewId(), "cookie1", "value1", domain, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie2", "value2", domain, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie3", "value3", domain, "/"),
        };

        _dbContext.Cookies.AddRange(cookies);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCookiesByDomainAsync(domain, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cookies);
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenNoCookiesExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        var domain = "test-domain";
        var otherDomain = "other-domain";
        var cookies = new[]
        {
            new CookieReadModel(CookieId.NewId(), "cookie1", "value1", otherDomain, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie2", "value2", otherDomain, "/"),
        };

        _dbContext.Cookies.AddRange(cookies);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCookiesByDomainAsync(domain, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var domain = "test-domain";
        var cookies = new[]
        {
            new CookieReadModel(CookieId.NewId(), "cookie1", "value1", domain, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie2", "value2", domain, "/"),
        };

        _dbContext.Cookies.AddRange(cookies);
        await _dbContext.SaveChangesAsync();

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => _sut.GetCookiesByDomainAsync(domain, cts.Token)
        );
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenMultipleDomainsExist_ShouldOnlyReturnCookiesForSpecifiedDomain()
    {
        // Arrange
        var domain1 = "domain1.com";
        var domain2 = "domain2.com";
        var cookies = new[]
        {
            new CookieReadModel(CookieId.NewId(), "cookie1", "value1", domain1, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie2", "value2", domain1, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie3", "value3", domain2, "/"),
            new CookieReadModel(CookieId.NewId(), "cookie4", "value4", domain2, "/"),
        };

        _dbContext.Cookies.AddRange(cookies);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCookiesByDomainAsync(domain1, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(cookie => cookie.Domain == domain1);
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
