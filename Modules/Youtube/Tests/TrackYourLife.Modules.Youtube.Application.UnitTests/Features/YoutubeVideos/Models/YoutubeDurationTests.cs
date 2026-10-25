using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Models;

public sealed class YoutubeDurationTests
{
    [Theory]
    [InlineData("PT4M12S", 252)]
    [InlineData("PT1H2M3S", 3723)]
    [InlineData("PT90S", 90)]
    public void ToSeconds_ReadsIsoDuration(string isoDuration, int expected)
    {
        YoutubeDuration.ToSeconds(isoDuration).Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("PT0S")]
    [InlineData("not-a-duration")]
    public void ToSeconds_RejectsMissingOrEmptyDurations(string? isoDuration)
    {
        YoutubeDuration.ToSeconds(isoDuration).Should().BeNull();
    }
}
