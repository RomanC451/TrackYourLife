using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;

internal sealed class GetChannelsByCategoryQueryValidator : AbstractValidator<GetChannelsByCategoryQuery>
{
    public GetChannelsByCategoryQueryValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .When(x => x.Category.HasValue)
            .WithMessage("Invalid video category.");
    }
}

