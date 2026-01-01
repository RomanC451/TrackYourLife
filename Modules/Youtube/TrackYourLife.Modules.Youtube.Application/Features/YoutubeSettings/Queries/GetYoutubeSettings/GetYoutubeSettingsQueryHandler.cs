using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

internal sealed class GetYoutubeSettingsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsQuery youtubeSettingsQuery
) : IQueryHandler<GetYoutubeSettingsQuery, YoutubeSettingReadModel?>
{
    public async Task<Result<YoutubeSettingReadModel?>> Handle(
        GetYoutubeSettingsQuery request,
        CancellationToken cancellationToken
    )
    {
        var settings = await youtubeSettingsQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(settings);
    }
}
