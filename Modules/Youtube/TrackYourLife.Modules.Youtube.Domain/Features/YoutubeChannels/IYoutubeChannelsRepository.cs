namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public interface IYoutubeChannelsRepository
{
    Task<YoutubeChannel?> GetByIdAsync(
        YoutubeChannelId id,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(YoutubeChannel channel, CancellationToken cancellationToken = default);

    void Remove(YoutubeChannel channel);

    void Update(YoutubeChannel channel);
}

