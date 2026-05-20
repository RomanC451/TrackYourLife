using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.DailyCategoryWatchCounters;

public class DailyCategoryWatchCounterTests
{
    private readonly DailyCategoryWatchCounterId _id = DailyCategoryWatchCounterId.NewId();
    private readonly UserId _userId = UserId.NewId();
    private readonly DateOnly _date = new(2026, 5, 14);
    private readonly YoutubeCategoryId _categoryId = YoutubeCategoryId.NewId();

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        var result = DailyCategoryWatchCounter.Create(_id, _userId, _date, _categoryId, 0);

        result.IsSuccess.Should().BeTrue();
        result.Value.VideosWatchedCount.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeCount_ShouldFail()
    {
        var result = DailyCategoryWatchCounter.Create(_id, _userId, _date, _categoryId, -1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Increment_ShouldIncreaseCount()
    {
        var counter = DailyCategoryWatchCounter.Create(_id, _userId, _date, _categoryId, 1).Value;

        counter.Increment().IsSuccess.Should().BeTrue();
        counter.VideosWatchedCount.Should().Be(2);
    }

    [Fact]
    public void CanWatchVideo_ShouldRespectMax()
    {
        var counter = DailyCategoryWatchCounter.Create(_id, _userId, _date, _categoryId, 2).Value;

        counter.CanWatchVideo(3).Should().BeTrue();
        counter.CanWatchVideo(2).Should().BeFalse();
    }
}
