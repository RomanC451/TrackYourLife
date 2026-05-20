using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;

public sealed record YoutubePolicyReadModel(
    YoutubeSettingReadModel? Settings,
    IReadOnlyList<YoutubeCategoryReadModel> Categories,
    IReadOnlyDictionary<YoutubeCategoryId, int>? SubscribedChannelCountsByCategoryId = null
);
