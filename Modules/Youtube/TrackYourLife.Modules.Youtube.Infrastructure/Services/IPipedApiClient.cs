namespace TrackYourLife.Modules.Youtube.Infrastructure.Services;

public interface IPipedApiClient
{
    Task<PipedPlaybackInfo> GetPlaybackInfoAsync(
        string videoId,
        CancellationToken cancellationToken = default
    );
}
