using System.Linq.Expressions;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels.Specifications;

internal sealed class YoutubeChannelReadModelWithUserIdAndYoutubeCategorySpecification(
    UserId userId,
    YoutubeCategoryId categoryId
) : Specification<YoutubeChannelReadModel, YoutubeChannelId>
{
    public override Expression<Func<YoutubeChannelReadModel, bool>> ToExpression() =>
        channel => channel.UserId == userId && channel.YoutubeCategoryId == categoryId;
}
