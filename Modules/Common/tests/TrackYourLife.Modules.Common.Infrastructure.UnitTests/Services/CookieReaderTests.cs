using System.Net;
using System.Text;
using Newtonsoft.Json;
using Serilog;
using TrackYourLife.Modules.Common.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Services;

public class CookiesReaderTest : IDisposable
{
    private readonly CookiesReader sut;
    private readonly ILogger logger;

    public CookiesReaderTest()
    {
        logger = Substitute.For<ILogger>();
        sut = new CookiesReader(logger);
    }

    [Fact]
    public async Task GetCookiesAsync_WhenCalledWithValidInputs_ReturnsCookies()
    {
        // Arrange
        var basePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..",
            "..",
            "..",
            "Services",
            "assets"
        );
        var cookieFilePath = Path.Combine(basePath, "Cookies");
        var localStateFilePath = Path.Combine(basePath, "Local State");

        var cookieFile = await File.ReadAllBytesAsync(cookieFilePath);
        var localStateFile = await File.ReadAllBytesAsync(localStateFilePath);

        string[] cookieDomains = [".myfitnesspal.com", "www.myfitnesspal.com"];
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await sut.GetCookiesAsync(
            cookieFile,
            localStateFile,
            cookieDomains,
            cancellationToken
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeOfType<List<Cookie>>();
    }

    [Fact]
    public async Task GetCookiesAsync_WhenCalledWithInvalidLocalStateFile_ReturnsFailedResult()
    {
        // Arrange
        var basePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..",
            "..",
            "..",
            "Services",
            "assets"
        );
        var cookieFilePath = Path.Combine(basePath, "Cookies");
        //var localStateFilePath = Path.Combine(basePath, "Local State");

        var cookieFile = await File.ReadAllBytesAsync(cookieFilePath);
        var localStateFile = Encoding.UTF8.GetBytes("invalid");

        string[] cookieDomains = [".myfitnesspal.com", "www.myfitnesspal.com"];
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await sut.GetCookiesAsync(
            cookieFile,
            localStateFile,
            cookieDomains,
            cancellationToken
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.CookieReader.FailedToGetKey);
    }

    [Fact]
    public async Task GetCookiesAsync_WhenCalledWithInvalidCookieFile_ReturnsFailedResult()
    {
        // Arrange
        var basePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..",
            "..",
            "..",
            "Services",
            "assets"
        );
        //var cookieFilePath = Path.Combine(basePath, "Cookies");
        var localStateFilePath = Path.Combine(basePath, "Local State");

        var cookieFile = Encoding.UTF8.GetBytes("invalid");

        var localStateFile = await File.ReadAllBytesAsync(localStateFilePath);

        string[] cookieDomains = [".myfitnesspal.com", "www.myfitnesspal.com"];
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await sut.GetCookiesAsync(
            cookieFile,
            localStateFile,
            cookieDomains,
            cancellationToken
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.CookieReader.FailedToReadCookies);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        logger.ClearSubstitute();
    }
}
