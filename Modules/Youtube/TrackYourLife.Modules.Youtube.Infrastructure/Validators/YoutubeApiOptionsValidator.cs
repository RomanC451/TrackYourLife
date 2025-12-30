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
    }
}
