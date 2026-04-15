using FluentValidation.TestHelper;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;
using TrackYourLife.Modules.Youtube.Infrastructure.Validators;

namespace TrackYourLife.Modules.Youtube.Infrastructure.UnitTests.Validators;

public class YoutubeApiOptionsValidatorTests
{
    private readonly YoutubeApiOptionsValidator _validator;

    public YoutubeApiOptionsValidatorTests()
    {
        _validator = new YoutubeApiOptionsValidator();
    }

    private static YoutubeApiOptions CreateValidOptions()
    {
        return new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
            PipedApiBaseUrl = "http://localhost:8080",
            PipedProxyBaseUrl = "http://localhost:8081",
            PipedFrontendBaseUrl = "http://localhost:3000",
        };
    }

    [Fact]
    public void Validate_WhenOptionsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = CreateValidOptions();

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Validate_WhenApiKeyIsEmpty_ShouldHaveValidationError(string? apiKey)
    {
        // Arrange
        var options = CreateValidOptions();
        options.ApiKey = apiKey!;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApiKey);
    }

    [Fact]
    public void Validate_WhenSearchCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.SearchCacheDuration = TimeSpan.Zero;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchCacheDuration);
    }

    [Fact]
    public void Validate_WhenSearchCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.SearchCacheDuration = TimeSpan.FromSeconds(-1);

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchCacheDuration);
    }

    [Fact]
    public void Validate_WhenChannelVideosCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.ChannelVideosCacheDuration = TimeSpan.Zero;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChannelVideosCacheDuration);
    }

    [Fact]
    public void Validate_WhenChannelVideosCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.ChannelVideosCacheDuration = TimeSpan.FromSeconds(-1);

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChannelVideosCacheDuration);
    }

    [Fact]
    public void Validate_WhenVideoDetailsCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.VideoDetailsCacheDuration = TimeSpan.Zero;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VideoDetailsCacheDuration);
    }

    [Fact]
    public void Validate_WhenVideoDetailsCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = CreateValidOptions();
        options.VideoDetailsCacheDuration = TimeSpan.FromSeconds(-1);

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VideoDetailsCacheDuration);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-url")]
    public void Validate_WhenPipedApiBaseUrlIsInvalid_ShouldHaveValidationError(string value)
    {
        // Arrange
        var options = CreateValidOptions();
        options.PipedApiBaseUrl = value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PipedApiBaseUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-url")]
    public void Validate_WhenPipedProxyBaseUrlIsInvalid_ShouldHaveValidationError(string value)
    {
        // Arrange
        var options = CreateValidOptions();
        options.PipedProxyBaseUrl = value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PipedProxyBaseUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("not-a-url")]
    public void Validate_WhenPipedFrontendBaseUrlIsInvalid_ShouldHaveValidationError(string value)
    {
        // Arrange
        var options = CreateValidOptions();
        options.PipedFrontendBaseUrl = value;

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PipedFrontendBaseUrl);
    }
}
