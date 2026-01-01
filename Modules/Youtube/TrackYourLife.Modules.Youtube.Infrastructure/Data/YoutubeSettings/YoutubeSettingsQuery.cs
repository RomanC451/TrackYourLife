using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeSettings.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeSettings;

internal sealed class YoutubeSettingsQuery(YoutubeReadDbContext dbContext)
    : GenericQuery<YoutubeSettingReadModel, YoutubeSettingsId>(dbContext.YoutubeSettings),
        IYoutubeSettingsQuery
{
    public async Task<YoutubeSettingReadModel?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await FirstOrDefaultAsync(
            new YoutubeSettingsReadModelWithUserIdSpecification(userId),
            cancellationToken
        );
    }
}
