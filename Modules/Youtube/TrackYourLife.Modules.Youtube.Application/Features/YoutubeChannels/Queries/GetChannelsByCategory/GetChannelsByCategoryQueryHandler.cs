using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;

internal sealed class GetChannelsByCategoryQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsQuery youtubeChannelsQuery
) : IQueryHandler<GetChannelsByCategoryQuery, IEnumerable<YoutubeChannelReadModel>>
{
    public async Task<Result<IEnumerable<YoutubeChannelReadModel>>> Handle(
        GetChannelsByCategoryQuery request,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<YoutubeChannelReadModel> channels;

        if (request.YoutubeCategoryId is { } categoryId)
        {
            channels = await youtubeChannelsQuery.GetByUserIdAndYoutubeCategoryIdAsync(
                userIdentifierProvider.UserId,
                categoryId,
                cancellationToken
            );
        }
        else
        {
            channels = await youtubeChannelsQuery.GetByUserIdAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
        }

        return Result.Success(channels);
    }
}
