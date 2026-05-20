using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

public sealed record GetYoutubeSettingsQuery() : IQuery<YoutubePolicyReadModel>;
