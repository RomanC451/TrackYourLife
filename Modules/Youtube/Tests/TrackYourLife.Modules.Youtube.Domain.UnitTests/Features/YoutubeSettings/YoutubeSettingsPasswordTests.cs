using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeSettings;

public class YoutubeSettingsPasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ShouldSucceed()
    {
        var result = YoutubeSettingsPassword.Create("ValidPass1!");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithTooShortPassword_ShouldFail()
    {
        var result = YoutubeSettingsPassword.Create("Short1!");

        result.IsFailure.Should().BeTrue();
    }
}
