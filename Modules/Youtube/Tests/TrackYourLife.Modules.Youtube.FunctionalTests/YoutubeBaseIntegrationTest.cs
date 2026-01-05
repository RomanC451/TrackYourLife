using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Youtube.Infrastructure.Data;
using TrackYourLife.SharedLib.FunctionalTests;

namespace TrackYourLife.Modules.Youtube.FunctionalTests;

[Collection("Youtube Integration Tests")]
public class YoutubeBaseIntegrationTest(YoutubeFunctionalTestWebAppFactory factory)
    : BaseIntegrationTest(factory, factory.GetCollection())
{
    protected YoutubeWriteDbContext _youtubeWriteDbContext =>
        _scope.ServiceProvider.GetRequiredService<YoutubeWriteDbContext>();

    protected YoutubeReadDbContext _youtubeReadDbContext =>
        _scope.ServiceProvider.GetRequiredService<YoutubeReadDbContext>();

    protected override async Task CleanupDatabaseAsync()
    {
        await CleanupDbSet(_youtubeWriteDbContext.OutboxMessages);
        await CleanupDbSet(_youtubeWriteDbContext.DailyEntertainmentCounters);
        await CleanupDbSet(_youtubeWriteDbContext.WatchedVideos);
        await CleanupDbSet(_youtubeWriteDbContext.YoutubeChannels);
        await CleanupDbSet(_youtubeWriteDbContext.YoutubeSettings);
    }

    protected async Task WaitForOutboxEventsToBeHandledAsync(
        CancellationToken cancellationToken = default
    )
    {
        await WaitForOutboxEventsToBeHandledAsync(
            _youtubeWriteDbContext.OutboxMessages,
            cancellationToken
        );
    }
}
