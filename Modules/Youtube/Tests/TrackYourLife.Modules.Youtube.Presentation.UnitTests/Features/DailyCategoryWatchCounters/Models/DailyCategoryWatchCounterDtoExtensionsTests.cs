using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Models;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.DailyCategoryWatchCounters.Models;

public class DailyCategoryWatchCounterDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithReadModel_ShouldMapCorrectly()
    {
        var id = DailyCategoryWatchCounterId.NewId();
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var date = new DateOnly(2026, 5, 14);
        var readModel = new DailyCategoryWatchCounterReadModel(id, userId, date, catId, 4);

        var dto = readModel.ToDto();

        dto.Id.Should().Be(id.Value);
        dto.YoutubeCategoryId.Should().Be(catId.Value);
        dto.Date.Should().Be(date);
        dto.VideosWatchedCount.Should().Be(4);
    }
}
