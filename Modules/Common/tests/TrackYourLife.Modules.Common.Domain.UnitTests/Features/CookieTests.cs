using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Common.Domain.UnitTests.Features;

public class CookieTests
{
    [Fact]
    public void Create_WhenValidParameters_ShouldReturnSuccessResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = "TestValue";
        var domain = "example.com";
        var path = "/";

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Value.Should().Be(value);
        result.Value.Domain.Should().Be(domain);
        result.Value.Path.Should().Be(path);
    }

    [Fact]
    public void Create_WhenEmptyId_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.Empty;
        var name = "TestCookie";
        var value = "TestValue";
        var domain = "example.com";
        var path = "/";

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(id)));
    }

    [Fact]
    public void Create_WhenEmptyName_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.NewId();

        var name = string.Empty;
        var value = "TestValue";
        var domain = "example.com";
        var path = "/";

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(name)));
    }

    [Fact]
    public void Create_WhenEmptyValue_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = string.Empty;
        var domain = "example.com";
        var path = "/";

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(value)));
    }

    [Fact]
    public void Create_WhenEmptyDomain_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = "TestValue";
        var domain = string.Empty;
        var path = "/";

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(domain)));
    }

    [Fact]
    public void Create_WhenEmptyPath_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = "TestValue";
        var domain = "example.com";
        var path = string.Empty;

        // Act
        var result = Cookie.Create(id, name, value, domain, path);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(path)));
    }

    [Fact]
    public void UpdateValue_WhenValidValue_ShouldReturnSuccessResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = "TestValue";
        var domain = "example.com";
        var path = "/";
        var cookie = Cookie.Create(id, name, value, domain, path).Value;
        var newValue = "NewValue";

        // Act
        var result = cookie.UpdateValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        cookie.Value.Should().Be(newValue);
    }

    [Fact]
    public void UpdateValue_WhenEmptyValue_ShouldReturnFailureResult()
    {
        // Arrange
        var id = CookieId.NewId();
        var name = "TestCookie";
        var value = "TestValue";
        var domain = "example.com";
        var path = "/";
        var cookie = Cookie.Create(id, name, value, domain, path).Value;
        var newValue = string.Empty;

        // Act
        var result = cookie.UpdateValue(newValue);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DomainErrors.ArgumentError.Empty(nameof(Cookie), nameof(value)));
    }
}
