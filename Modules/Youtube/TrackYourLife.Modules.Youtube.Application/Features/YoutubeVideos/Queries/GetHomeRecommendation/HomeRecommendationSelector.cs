using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;

internal static class HomeRecommendationSelector
{
    public static YoutubeVideoPreview? Pick(
        IReadOnlyList<IReadOnlyList<YoutubeVideoPreview>> videosByChannel,
        Random? random = null
    )
    {
        random ??= Random.Shared;

        var perChannelWinners = new List<YoutubeVideoPreview>();

        foreach (var channelVideos in videosByChannel)
        {
            var winner = PickFromChannel(channelVideos, random);
            if (winner is not null)
            {
                perChannelWinners.Add(winner);
            }
        }

        if (perChannelWinners.Count == 0)
        {
            return null;
        }

        return perChannelWinners[random.Next(perChannelWinners.Count)];
    }

    internal static YoutubeVideoPreview? PickFromChannel(
        IReadOnlyList<YoutubeVideoPreview> channelVideos,
        Random random
    )
    {
        if (channelVideos.Count == 0)
        {
            return null;
        }

        var candidates = channelVideos
            .OrderByDescending(video => video.PublishedAt)
            .Take(2)
            .ToList();

        var unwatched = candidates.Where(video => !video.IsWatched).ToList();
        var pool = unwatched.Count > 0 ? unwatched : candidates;

        return pool[random.Next(pool.Count)];
    }
}
