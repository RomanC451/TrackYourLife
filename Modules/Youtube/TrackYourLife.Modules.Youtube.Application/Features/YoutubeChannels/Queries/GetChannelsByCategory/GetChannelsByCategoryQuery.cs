using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;

public sealed record GetChannelsByCategoryQuery(
    YoutubeCategoryId? YoutubeCategoryId,
    bool FavoritesOnly = false
) : IQuery<IEnumerable<YoutubeChannelReadModel>>;
