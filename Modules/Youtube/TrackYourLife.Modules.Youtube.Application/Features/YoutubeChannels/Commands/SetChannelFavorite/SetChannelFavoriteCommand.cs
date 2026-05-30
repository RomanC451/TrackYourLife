using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;

public sealed record SetChannelFavoriteCommand(string YoutubeChannelId, bool IsFavorite) : ICommand;
