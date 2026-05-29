using TrackYourLife.Modules.Youtube.Application.Core.Security;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Core.Security;

public sealed class YoutubeSettingsPasswordGeneratorTests
{
    [Fact]
    public void Generate_ShouldMeetPasswordRules()
    {
        var result = YoutubeSettingsPasswordGenerator.Generate();

        result.IsSuccess.Should().BeTrue();
        result.Value.Length.Should().BeGreaterOrEqualTo(YoutubeSettingsPassword.MinLength);
        YoutubeSettingsPassword.Create(result.Value).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Generate_ManyIterations_ShouldAlwaysSucceed()
    {
        for (var i = 0; i < 50; i++)
        {
            var result = YoutubeSettingsPasswordGenerator.Generate();
            result.IsSuccess.Should().BeTrue();
        }
    }
}
