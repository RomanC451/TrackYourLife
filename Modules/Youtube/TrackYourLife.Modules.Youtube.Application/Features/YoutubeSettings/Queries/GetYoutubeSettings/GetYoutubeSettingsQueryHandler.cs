using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;
using MediatR;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

internal sealed class GetYoutubeSettingsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsQuery youtubeSettingsQuery,
    IYoutubeCategoriesQuery youtubeCategoriesQuery,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    ISender sender
) : IQueryHandler<GetYoutubeSettingsQuery, YoutubePolicyReadModel>
{
    public async Task<Result<YoutubePolicyReadModel>> Handle(
        GetYoutubeSettingsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var settings = await youtubeSettingsQuery.GetByUserIdAsync(userId, cancellationToken);
        var categories = await youtubeCategoriesQuery.ListByUserIdOrderedAsync(userId, cancellationToken);

        if (categories.Count == 0)
        {
            var seed = await sender.Send(
                new EnsureDefaultYoutubeCategoriesCommand(userId),
                cancellationToken
            );
            if (seed.IsFailure)
            {
                return Result.Failure<YoutubePolicyReadModel>(seed.Error);
            }

            categories = await youtubeCategoriesQuery.ListByUserIdOrderedAsync(userId, cancellationToken);
        }

        var channelCounts = await youtubeChannelsQuery.CountByUserIdGroupedByCategoryAsync(
            userId,
            cancellationToken
        );

        return Result.Success(new YoutubePolicyReadModel(settings, categories, channelCounts));
    }
}
