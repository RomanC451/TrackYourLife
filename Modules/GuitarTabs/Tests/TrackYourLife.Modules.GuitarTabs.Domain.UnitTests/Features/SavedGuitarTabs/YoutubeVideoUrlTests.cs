using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Domain.UnitTests.Features.SavedGuitarTabs;

public sealed class YoutubeVideoUrlTests
{
    [Theory]
    [InlineData("https://youtu.be/V1OfCmY-ZRw?is=q5HSM6Js4Gp0Jy-s")]
    [InlineData("https://youtu.be/V1OfCmY-ZRw?si=q5HSM6Js4Gp0Jy-s")]
    [InlineData("https://youtu.be/RBumgq5yVrA?t=12")]
    [InlineData("https://www.youtube.com/watch?v=V1OfCmY-ZRw&is=q5HSM6Js4Gp0Jy-s")]
    public void TryNormalize_AcceptsShareLinksWithQueryParameters(string url)
    {
        var normalized = YoutubeVideoUrl.TryNormalize(url, out var value);

        normalized.Should().BeTrue();
        value.Should().Be(
            url.Contains("RBumgq5yVrA", StringComparison.Ordinal)
                ? "https://www.youtube.com/watch?v=RBumgq5yVrA"
                : "https://www.youtube.com/watch?v=V1OfCmY-ZRw"
        );
    }
}
