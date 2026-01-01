using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

public sealed record GetYoutubeSettingsQuery() : IQuery<YoutubeSettingReadModel?>;
