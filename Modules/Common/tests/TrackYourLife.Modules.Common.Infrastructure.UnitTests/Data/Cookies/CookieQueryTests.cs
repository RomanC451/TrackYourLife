using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Data.Cookies;

public class CookieQueryTests : IDisposable
{
    private readonly CookieQuery sut;
    private readonly CommonReadDbContext dbContext;

    public CookieQueryTests()
    {
        var options = new DbContextOptionsBuilder<CommonReadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        dbContext = new CommonReadDbContext(options, null!);
        sut = new CookieQuery(dbContext);
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenCookiesExist_ReturnsCookies()
    {
        // Arrange
        var domain = "example.com";
        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name1", "value1", domain, "/"),
            new(CookieId.NewId(), "name2", "value2", domain, "/"),
            new(CookieId.NewId(), "name3", "value3", domain, "/"),
        };
        dbContext.Cookies.AddRange(cookies);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetCookiesByDomainAsync(domain, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cookies);
    }

    [Fact]
    public async Task GetCookiesByDomainAsync_WhenCookiesNotFound_ReturnsEmptyList()
    {
        // Arrange

        var cookies = new List<CookieReadModel>
        {
            new(CookieId.NewId(), "name1", "value1", "example.com", "/"),
            new(CookieId.NewId(), "name2", "value2", "example.com", "/"),
            new(CookieId.NewId(), "name3", "value3", "example.com", "/"),
        };
        dbContext.Cookies.AddRange(cookies);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetCookiesByDomainAsync("domain", CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        dbContext.Dispose();
    }
}
