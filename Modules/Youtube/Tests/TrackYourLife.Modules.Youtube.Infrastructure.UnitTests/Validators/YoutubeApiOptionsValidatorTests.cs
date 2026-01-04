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

    [Fact]
    public void Validate_WhenOptionsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

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
        var options = new YoutubeApiOptions
        {
            ApiKey = apiKey!,
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApiKey);
    }

    [Fact]
    public void Validate_WhenSearchCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.Zero,
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchCacheDuration);
    }

    [Fact]
    public void Validate_WhenSearchCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromSeconds(-1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchCacheDuration);
    }

    [Fact]
    public void Validate_WhenChannelVideosCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.Zero,
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChannelVideosCacheDuration);
    }

    [Fact]
    public void Validate_WhenChannelVideosCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromSeconds(-1),
            VideoDetailsCacheDuration = TimeSpan.FromHours(2),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ChannelVideosCacheDuration);
    }

    [Fact]
    public void Validate_WhenVideoDetailsCacheDurationIsZero_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.Zero,
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VideoDetailsCacheDuration);
    }

    [Fact]
    public void Validate_WhenVideoDetailsCacheDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var options = new YoutubeApiOptions
        {
            ApiKey = "test-api-key",
            SearchCacheDuration = TimeSpan.FromHours(1),
            ChannelVideosCacheDuration = TimeSpan.FromMinutes(30),
            VideoDetailsCacheDuration = TimeSpan.FromSeconds(-1),
        };

        // Act
        var result = _validator.TestValidate(options);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.VideoDetailsCacheDuration);
    }
}
