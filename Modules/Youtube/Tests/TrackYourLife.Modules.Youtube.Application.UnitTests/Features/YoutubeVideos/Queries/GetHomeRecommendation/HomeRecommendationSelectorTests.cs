using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetHomeRecommendation;

public sealed class HomeRecommendationSelectorTests
{
    private static readonly DateTime BaseDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void PickFromChannel_WhenOneUnwatchedAmongTwo_PicksUnwatched()
    {
        var videos = new List<YoutubeVideoPreview>
        {
            CreateVideo("newer-watched", BaseDate.AddDays(1), isWatched: true),
            CreateVideo("older-unwatched", BaseDate, isWatched: false),
        };

        var result = HomeRecommendationSelector.PickFromChannel(videos, new Random(0));

        result!.VideoId.Should().Be("older-unwatched");
    }

    [Fact]
    public void PickFromChannel_WhenBothWatched_PicksOneOfTheTwo()
    {
        var videos = new List<YoutubeVideoPreview>
        {
            CreateVideo("video-a", BaseDate.AddDays(1), isWatched: true),
            CreateVideo("video-b", BaseDate, isWatched: true),
        };

        var result = HomeRecommendationSelector.PickFromChannel(videos, new Random(0));

        result.Should().NotBeNull();
        result!.VideoId.Should().BeOneOf("video-a", "video-b");
    }

    [Fact]
    public void PickFromChannel_WhenOnlyOneVideo_ReturnsIt()
    {
        var videos = new List<YoutubeVideoPreview>
        {
            CreateVideo("only-one", BaseDate, isWatched: false),
        };

        var result = HomeRecommendationSelector.PickFromChannel(videos, new Random(0));

        result!.VideoId.Should().Be("only-one");
    }

    [Fact]
    public void Pick_WhenMultipleChannels_ReturnsOneWinner()
    {
        var videosByChannel = new List<IReadOnlyList<YoutubeVideoPreview>>
        {
            new List<YoutubeVideoPreview> { CreateVideo("c1-v1", BaseDate, isWatched: false, channelId: "c1") },
            new List<YoutubeVideoPreview> { CreateVideo("c2-v1", BaseDate, isWatched: false, channelId: "c2") },
        };

        var result = HomeRecommendationSelector.Pick(videosByChannel, new Random(0));

        result.Should().NotBeNull();
        result!.VideoId.Should().BeOneOf("c1-v1", "c2-v1");
    }

    [Fact]
    public void Pick_WhenNoVideos_ReturnsNull()
    {
        var videosByChannel = new List<IReadOnlyList<YoutubeVideoPreview>>
        {
            new List<YoutubeVideoPreview>(),
            new List<YoutubeVideoPreview>(),
        };

        var result = HomeRecommendationSelector.Pick(videosByChannel, new Random(0));

        result.Should().BeNull();
    }

    private static YoutubeVideoPreview CreateVideo(
        string videoId,
        DateTime publishedAt,
        bool isWatched,
        string channelId = "channel-1"
    ) =>
        new(
            videoId,
            "Title",
            "thumbnail",
            "Channel",
            channelId,
            publishedAt,
            "PT10M",
            100,
            isWatched
        );
}
