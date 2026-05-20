using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryMetadata;

public sealed record UpdateYoutubeCategoryMetadataCommand(
    YoutubeCategoryId CategoryId,
    string Name,
    int DisplayOrder
) : ICommand;
