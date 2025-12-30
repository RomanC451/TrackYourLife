using System.Linq.Expressions;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels.Specifications;

internal sealed class YoutubeChannelReadModelWithUserIdAndCategorySpecification(
    UserId userId,
    VideoCategory category
) : Specification<YoutubeChannelReadModel, YoutubeChannelId>
{
    public override Expression<Func<YoutubeChannelReadModel, bool>> ToExpression() =>
        channel => channel.UserId == userId && channel.Category == category;
}

