using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;

public sealed record RemoveChannelCommand(string YoutubeChannelId) : ICommand;
