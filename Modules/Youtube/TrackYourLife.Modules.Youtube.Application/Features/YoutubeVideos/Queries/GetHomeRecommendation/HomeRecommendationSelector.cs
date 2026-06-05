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

        var allChannelsCaughtUp = videosByChannel.All(IsChannelCaughtUp);

        var perChannelWinners = new List<YoutubeVideoPreview>();

        foreach (var channelVideos in videosByChannel)
        {
            var winner = allChannelsCaughtUp
                ? PickFromChannelIncludingWatched(channelVideos, random)
                : PickFromChannelUnwatchedOnly(channelVideos, random);

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

    internal static bool IsChannelCaughtUp(IReadOnlyList<YoutubeVideoPreview> channelVideos)
    {
        if (channelVideos.Count == 0)
        {
            return true;
        }

        return GetCandidates(channelVideos).All(video => video.IsWatched);
    }

    internal static YoutubeVideoPreview? PickFromChannelUnwatchedOnly(
        IReadOnlyList<YoutubeVideoPreview> channelVideos,
        Random random
    )
    {
        if (channelVideos.Count == 0)
        {
            return null;
        }

        var unwatched = GetCandidates(channelVideos).Where(video => !video.IsWatched).ToList();

        if (unwatched.Count == 0)
        {
            return null;
        }

        return unwatched[random.Next(unwatched.Count)];
    }

    internal static YoutubeVideoPreview? PickFromChannelIncludingWatched(
        IReadOnlyList<YoutubeVideoPreview> channelVideos,
        Random random
    )
    {
        if (channelVideos.Count == 0)
        {
            return null;
        }

        var candidates = GetCandidates(channelVideos);

        return candidates[random.Next(candidates.Count)];
    }

    private static List<YoutubeVideoPreview> GetCandidates(
        IReadOnlyList<YoutubeVideoPreview> channelVideos
    ) => channelVideos.OrderByDescending(video => video.PublishedAt).Take(2).ToList();
}
