using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Common.Infrastructure.Data.Cookies;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Data.Cookies;

public class CookieRepositoryTests : IDisposable
{
    private readonly CookieRepository sut;
    private readonly CommonWriteDbContext dbContext;

    public CookieRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<CommonWriteDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new CommonWriteDbContext(options, null!);
        sut = new CookieRepository(dbContext);
    }

    [Fact]
    public async Task GetByNameAndDomainAsync_WhenCookieExists_ReturnsCookie()
    {
        // Arrange
        var cookie = Cookie.Create(CookieId.NewId(), "name", "value", "example.com", "/").Value;
        dbContext.Cookies.Add(cookie);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await sut.GetByNameAndDomainAsync(
            cookie.Name,
            cookie.Domain,
            CancellationToken.None
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(cookie);
    }

    [Fact]
    public async Task GetByNameAndDomainAsync_WhenCookieNotFound_ReturnsNull()
    {
        // Arrange
        var cookies = new List<Cookie>
        {
            Cookie.Create(CookieId.NewId(), "name1", "value1", "example.com", "/").Value,
            Cookie.Create(CookieId.NewId(), "name2", "value2", "example.com", "/").Value,
            Cookie.Create(CookieId.NewId(), "name3", "value3", "example.com", "/").Value
        };
        dbContext.Cookies.AddRange(cookies);
        await dbContext.SaveChangesAsync();
        // Act
        var result = await sut.GetByNameAndDomainAsync(
            "nonexistent",
            "example.com",
            CancellationToken.None
        );

        // Assert
        result.Should().BeNull();
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
