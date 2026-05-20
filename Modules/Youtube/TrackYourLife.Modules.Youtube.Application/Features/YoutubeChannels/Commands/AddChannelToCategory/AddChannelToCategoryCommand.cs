using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;

public sealed record AddChannelToCategoryCommand(
    string YoutubeChannelId,
    YoutubeCategoryId YoutubeCategoryId
) : ICommand<YoutubeChannelId>;
