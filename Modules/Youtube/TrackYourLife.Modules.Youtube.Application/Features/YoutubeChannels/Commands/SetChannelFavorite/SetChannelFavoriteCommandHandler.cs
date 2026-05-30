using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.SetChannelFavorite;

internal sealed class SetChannelFavoriteCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<SetChannelFavoriteCommand>
{
    public async Task<Result> Handle(
        SetChannelFavoriteCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var channel = await youtubeChannelsRepository.GetByUserIdAndYoutubeChannelIdAsync(
            userId,
            request.YoutubeChannelId,
            cancellationToken
        );

        if (channel is null)
        {
            return Result.Failure(YoutubeChannelsErrors.NotFound(request.YoutubeChannelId));
        }

        if (channel.IsFavorite == request.IsFavorite)
        {
            return Result.Success();
        }

        channel.SetFavorite(request.IsFavorite, dateTimeProvider.UtcNow);
        youtubeChannelsRepository.Update(channel);

        return Result.Success();
    }
}
