using FluentAssertions;
using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Validators;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Validators;

public class FoodApiOptionsValidatorTests
{
    private readonly FoodApiOptionsValidator _validator;

    public FoodApiOptionsValidatorTests()
    {
        _validator = new FoodApiOptionsValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldNotHaveValidationError()
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public void Validate_WithInvalidBaseUrl_ShouldHaveValidationError(string? baseUrl)
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = baseUrl!,
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BaseUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public void Validate_WithInvalidBaseApiUrl_ShouldHaveValidationError(string? baseApiUrl)
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = baseApiUrl!,
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BaseApiUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidSearchPath_ShouldHaveValidationError(string? searchPath)
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = searchPath!,
            AuthTokenPath = "/auth",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchPath);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidAuthTokenPath_ShouldHaveValidationError(string? authTokenPath)
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = authTokenPath!,
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthTokenPath);
    }

    [Fact]
    public void Validate_WithNullCookieDomains_ShouldHaveValidationError()
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = null!,
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CookieDomains);
    }

    [Fact]
    public void Validate_WithEmptyCookieDomains_ShouldHaveValidationError()
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = Array.Empty<string>(),
            SpaceEncoded = "%20",
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CookieDomains);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithInvalidSpaceEncoded_ShouldHaveValidationError(string? spaceEncoded)
    {
        // Arrange
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/v1",
            SearchPath = "/search",
            AuthTokenPath = "/auth",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = spaceEncoded!,
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SpaceEncoded);
    }
}
