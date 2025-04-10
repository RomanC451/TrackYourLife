using System.Linq;
using System.Net;
using FluentAssertions;
using TrackYourLife.Modules.Common.Infrastructure.Services;
using Xunit;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Services;

public class CookiesReaderTests
{
    private readonly CookiesReader _sut;

    public CookiesReaderTests()
    {
        _sut = new CookiesReader();
    }

    [Fact]
    public async Task GetCookiesAsync_WithValidCookieFile_ShouldReturnCookies()
    {
        // Arrange
        var cookieFileContent =
            @"# Netscape HTTP Cookie File
# https://curl.haxx.se/rfc/cookie_spec.html
# This is a generated file!  Do not edit.

.example.com	TRUE	/	FALSE	1735689600	sessionId	abc123
.example.com	TRUE	/	TRUE	1735689600	secureCookie	xyz789
.example.com	TRUE	/api	FALSE	0	persistentCookie	def456";

        var cookieFileBytes = System.Text.Encoding.UTF8.GetBytes(cookieFileContent);

        // Act
        var result = await _sut.GetCookiesAsync(cookieFileBytes, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().HaveCount(3);

        var sessionCookie = result.Value.First(c => c.Name == "sessionId");
        sessionCookie.Should().NotBeNull();
        sessionCookie.Value.Should().Be("abc123");
        sessionCookie.Domain.Should().Be(".example.com");
        sessionCookie.Path.Should().Be("/");
        sessionCookie.Secure.Should().BeFalse();
        sessionCookie.Expires.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1735689600).DateTime);

        var secureCookie = result.Value.First(c => c.Name == "secureCookie");
        secureCookie.Should().NotBeNull();
        secureCookie.Value.Should().Be("xyz789");
        secureCookie.Secure.Should().BeTrue();

        var persistentCookie = result.Value.First(c => c.Name == "persistentCookie");
        persistentCookie.Should().NotBeNull();
        persistentCookie.Value.Should().Be("def456");
        persistentCookie.Path.Should().Be("/api");
        persistentCookie.Expires.Should().Be(DateTime.MinValue);
    }

    [Fact]
    public async Task GetCookiesAsync_WithEmptyFile_ShouldReturnEmptyList()
    {
        // Arrange
        var cookieFileBytes = Array.Empty<byte>();

        // Act
        var result = await _sut.GetCookiesAsync(cookieFileBytes, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCookiesAsync_WithOnlyCommentsAndEmptyLines_ShouldReturnEmptyList()
    {
        // Arrange
        var cookieFileContent =
            @"# Netscape HTTP Cookie File
# https://curl.haxx.se/rfc/cookie_spec.html
# This is a generated file!  Do not edit.

";

        var cookieFileBytes = System.Text.Encoding.UTF8.GetBytes(cookieFileContent);

        // Act
        var result = await _sut.GetCookiesAsync(cookieFileBytes, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCookiesAsync_WithInvalidLineFormat_ShouldSkipInvalidLines()
    {
        // Arrange
        var cookieFileContent =
            @"# Netscape HTTP Cookie File
.example.com	TRUE	/	FALSE	1735689600	sessionId	abc123
invalid-line
.example.com	TRUE	/	TRUE	1735689600	secureCookie	xyz789
another-invalid-line
";

        var cookieFileBytes = System.Text.Encoding.UTF8.GetBytes(cookieFileContent);

        // Act
        var result = await _sut.GetCookiesAsync(cookieFileBytes, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Select(c => c.Name).Should().BeEquivalentTo("sessionId", "secureCookie");
    }

    [Fact]
    public async Task GetCookiesAsync_WithCancellation_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var cookieFileContent = ".example.com	TRUE	/	FALSE	1735689600	sessionId	abc123";
        var cookieFileBytes = System.Text.Encoding.UTF8.GetBytes(cookieFileContent);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await _sut.Invoking(x => x.GetCookiesAsync(cookieFileBytes, cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }
}
