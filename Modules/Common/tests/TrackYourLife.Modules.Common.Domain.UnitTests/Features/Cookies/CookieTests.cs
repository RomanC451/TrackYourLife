using FluentAssertions;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Common.Domain.UnitTests.Features.Cookies;

public class CookieTests
{
    private static readonly CookieId ValidId = CookieId.NewId();
    private const string ValidName = "TestCookie";
    private const string ValidValue = "TestValue";
    private const string ValidDomain = "test.com";
    private const string ValidPath = "/";

    [Fact]
    public void Create_WithValidParameters_ShouldCreateCookie()
    {
        // Act
        var result = Cookie.Create(ValidId, ValidName, ValidValue, ValidDomain, ValidPath);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(ValidId);
        result.Value.Name.Should().Be(ValidName);
        result.Value.Value.Should().Be(ValidValue);
        result.Value.Domain.Should().Be(ValidDomain);
        result.Value.Path.Should().Be(ValidPath);
    }

    [Theory]
    [MemberData(nameof(GetInvalidParameters))]
    public void Create_WithInvalidParameters_ShouldReturnFailure(
        string id,
        string name,
        string value,
        string domain,
        string path
    )
    {
        // Act
        var cookieId = id == "empty" ? new CookieId(Guid.Empty) : new CookieId(Guid.Parse(id));
        var result = Cookie.Create(cookieId, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
    }

    public static TheoryData<string, string, string, string, string> GetInvalidParameters()
    {
        return new TheoryData<string, string, string, string, string>
        {
            { "empty", ValidName, ValidValue, ValidDomain, ValidPath },
            { ValidId.Value.ToString(), null!, ValidValue, ValidDomain, ValidPath },
            { ValidId.Value.ToString(), "", ValidValue, ValidDomain, ValidPath },
            { ValidId.Value.ToString(), ValidName, null!, ValidDomain, ValidPath },
            { ValidId.Value.ToString(), ValidName, "", ValidDomain, ValidPath },
            { ValidId.Value.ToString(), ValidName, ValidValue, null!, ValidPath },
            { ValidId.Value.ToString(), ValidName, ValidValue, "", ValidPath },
            { ValidId.Value.ToString(), ValidName, ValidValue, ValidDomain, null! },
            { ValidId.Value.ToString(), ValidName, ValidValue, ValidDomain, "" },
        };
    }

    [Fact]
    public void UpdateValue_WithValidValue_ShouldUpdateCookieValue()
    {
        // Arrange
        var cookie = Cookie.Create(ValidId, ValidName, ValidValue, ValidDomain, ValidPath).Value;
        var newValue = "NewValue";

        // Act
        var result = cookie.UpdateValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        cookie.Value.Should().Be(newValue);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void UpdateValue_WithInvalidValue_ShouldReturnFailure(string? invalidValue)
    {
        // Arrange
        var cookie = Cookie.Create(ValidId, ValidName, ValidValue, ValidDomain, ValidPath).Value;

        // Act
        var result = cookie.UpdateValue(invalidValue ?? string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
        cookie.Value.Should().Be(ValidValue);
    }
}
