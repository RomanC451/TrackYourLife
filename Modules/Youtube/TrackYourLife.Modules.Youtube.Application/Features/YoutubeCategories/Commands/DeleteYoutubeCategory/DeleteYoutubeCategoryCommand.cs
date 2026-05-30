using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;

public sealed record DeleteYoutubeCategoryCommand(
    YoutubeCategoryId CategoryId,
    bool ConfirmUnsubscribeChannels,
    YoutubeCategoryId? MoveChannelsToCategoryId
) : ICommand;
