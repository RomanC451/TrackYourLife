using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Validators;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Validators;

public class FoodApiOptionsValidatorTests
{
    private readonly FoodApiOptionsValidator validator = new();

    [Fact]
    public void When_ValidData_ShouldPassValidation()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void When_BaseUrlIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = string.Empty,
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.BaseUrl);
    }

    [Fact]
    public void When_BaseUrlIsInvalid_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "invalid-url",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.BaseUrl);
    }

    [Fact]
    public void When_BaseApiUrlIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = string.Empty,
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.BaseApiUrl);
    }

    [Fact]
    public void When_BaseApiUrlIsInvalid_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "invalid-url",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.BaseApiUrl);
    }

    [Fact]
    public void When_SearchPathIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = string.Empty,
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.SearchPath);
    }

    [Fact]
    public void When_AuthTokenPathIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = string.Empty,
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.AuthTokenPath);
    }

    [Fact]
    public void When_CookieDomainsIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = Array.Empty<string>(),
            SpaceEncoded = "+"
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.CookieDomains);
    }

    [Fact]
    public void When_SpaceEncodedIsEmpty_ShouldFail()
    {
        var options = new FoodApiOptions
        {
            BaseUrl = "https://api.example.com",
            BaseApiUrl = "https://api.example.com/api",
            SearchPath = "/search",
            AuthTokenPath = "/auth/token",
            CookieDomains = new[] { "example.com" },
            SpaceEncoded = string.Empty
        };

        validator.TestValidate(options).ShouldHaveValidationErrorFor(x => x.SpaceEncoded);
    }
}
