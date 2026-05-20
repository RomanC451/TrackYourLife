using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;

public sealed record UpdateYoutubeCategoryLimitCommand(
    YoutubeCategoryId CategoryId,
    int MaxVideosPerDay
) : ICommand;
