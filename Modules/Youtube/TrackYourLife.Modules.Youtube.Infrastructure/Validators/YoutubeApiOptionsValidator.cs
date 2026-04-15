using FluentValidation;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Validators;

/// <summary>
/// Validates the options for the YouTube API.
/// </summary>
internal sealed class YoutubeApiOptionsValidator : AbstractValidator<YoutubeApiOptions>
{
    public YoutubeApiOptionsValidator()
    {
        RuleFor(x => x.ApiKey).NotEmpty().WithMessage("YouTube API key is required.");

        RuleFor(x => x.SearchCacheDuration)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Search cache duration must be greater than zero.");

        RuleFor(x => x.ChannelVideosCacheDuration)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Channel videos cache duration must be greater than zero.");

        RuleFor(x => x.VideoDetailsCacheDuration)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Video details cache duration must be greater than zero.");

        RuleFor(x => x.PipedApiBaseUrl)
            .NotEmpty()
            .Must(BeValidUrl)
            .WithMessage("Piped API base URL must be a valid absolute URL.");

        RuleFor(x => x.PipedProxyBaseUrl)
            .NotEmpty()
            .Must(BeValidUrl)
            .WithMessage("Piped proxy base URL must be a valid absolute URL.");

        RuleFor(x => x.PipedFrontendBaseUrl)
            .NotEmpty()
            .Must(BeValidUrl)
            .WithMessage("Piped frontend base URL must be a valid absolute URL.");
    }

    private static bool BeValidUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out _);
    }
}
