using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeCategories;

public class YoutubeCategoryIdTests
{
    [Fact]
    public void YoutubeCategoryId_ShouldInheritFromStronglyTypedGuid()
    {
        var idType = typeof(YoutubeCategoryId);
        var baseType = typeof(StronglyTypedGuid<YoutubeCategoryId>);

        idType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void TryParse_WithValidGuid_ShouldSucceed()
    {
        var g = Guid.NewGuid();

        var ok = YoutubeCategoryId.TryParse(g.ToString(), out var id);

        ok.Should().BeTrue();
        id!.Value.Should().Be(g);
    }
}
