using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

internal sealed class SearchYoutubeChannelsQueryValidator : AbstractValidator<SearchYoutubeChannelsQuery>
{
    public SearchYoutubeChannelsQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .WithMessage("Search query is required.")
            .MaximumLength(100)
            .WithMessage("Search query must not exceed 100 characters.");

        RuleFor(x => x.MaxResults)
            .InclusiveBetween(1, 50)
            .WithMessage("Max results must be between 1 and 50.");
    }
}

