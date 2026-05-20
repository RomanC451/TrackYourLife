using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;

internal sealed class GetChannelsByCategoryQueryValidator : AbstractValidator<GetChannelsByCategoryQuery>
{
    public GetChannelsByCategoryQueryValidator()
    {
        When(
            x => x.YoutubeCategoryId is not null,
            () =>
            {
                RuleFor(x => x.YoutubeCategoryId!).NotEqual(YoutubeCategoryId.Empty).WithMessage("Invalid category id.");
            }
        );
    }
}
