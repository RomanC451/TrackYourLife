using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;

public sealed record RemoveChannelCommand(YoutubeChannelId Id) : ICommand;

