using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

public sealed record YoutubeCategoryReadModel(
    YoutubeCategoryId Id,
    UserId UserId,
    string Name,
    int MaxVideosPerDay,
    int DisplayOrder,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<YoutubeCategoryId>;
