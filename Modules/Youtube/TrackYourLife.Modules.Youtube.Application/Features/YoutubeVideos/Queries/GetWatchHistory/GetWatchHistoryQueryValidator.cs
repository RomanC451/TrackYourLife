using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;

internal sealed class GetWatchHistoryQueryValidator : AbstractValidator<GetWatchHistoryQuery>
{
    public GetWatchHistoryQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);

        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(50);
    }
}
