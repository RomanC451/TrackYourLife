using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.DailyCategoryWatchCounters;

public class DailyCategoryWatchCounterIdTests
{
    [Fact]
    public void DailyCategoryWatchCounterId_ShouldInheritFromStronglyTypedGuid()
    {
        var idType = typeof(DailyCategoryWatchCounterId);
        var baseType = typeof(StronglyTypedGuid<DailyCategoryWatchCounterId>);

        idType.Should().BeAssignableTo(baseType);
    }

    [Fact]
    public void TryParse_WithValidGuid_ShouldSucceed()
    {
        var g = Guid.NewGuid();

        var ok = DailyCategoryWatchCounterId.TryParse(g.ToString(), out var id);

        ok.Should().BeTrue();
        id!.Value.Should().Be(g);
    }
}
