using System.Linq.Expressions;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeSettings.Specifications;

internal sealed class YoutubeSettingsReadModelWithUserIdSpecification(UserId userId)
    : Specification<YoutubeSettingReadModel, YoutubeSettingsId>
{
    public override Expression<Func<YoutubeSettingReadModel, bool>> ToExpression() =>
        settings => settings.UserId == userId;
}
