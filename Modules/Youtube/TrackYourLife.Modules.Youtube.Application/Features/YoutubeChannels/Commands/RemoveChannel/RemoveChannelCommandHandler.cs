using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.RemoveChannel;

internal sealed class RemoveChannelCommandHandler(
    IYoutubeChannelsRepository youtubeChannelsRepository
) : ICommandHandler<RemoveChannelCommand>
{
    public async Task<Result> Handle(
        RemoveChannelCommand request,
        CancellationToken cancellationToken
    )
    {
        var channel = await youtubeChannelsRepository.GetByIdAsync(request.Id, cancellationToken);

        if (channel is null)
        {
            return Result.Failure(YoutubeChannelsErrors.NotFound(request.Id));
        }

        youtubeChannelsRepository.Remove(channel);

        return Result.Success();
    }
}

