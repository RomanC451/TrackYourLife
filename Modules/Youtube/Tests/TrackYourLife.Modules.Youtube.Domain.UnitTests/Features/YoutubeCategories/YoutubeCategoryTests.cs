using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeCategories;

public class YoutubeCategoryTests
{
    private readonly YoutubeCategoryId _id = YoutubeCategoryId.NewId();
    private readonly UserId _userId = UserId.NewId();
    private readonly DateTime _createdOnUtc = DateTime.UtcNow;

    [Fact]
    public void Create_WithValidParameters_ShouldSucceed()
    {
        var result = YoutubeCategory.Create(_id, _userId, "Entertainment", 5, 0, _createdOnUtc);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Entertainment");
        result.Value.MaxVideosPerDay.Should().Be(5);
        result.Value.DisplayOrder.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        var result = YoutubeCategory.Create(_id, _userId, "  ", 5, 0, _createdOnUtc);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.NameRequired);
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldFail()
    {
        var name = new string('a', YoutubeCategory.MaxNameLength + 1);

        var result = YoutubeCategory.Create(_id, _userId, name, 5, 0, _createdOnUtc);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.NameTooLong);
    }

    [Fact]
    public void Create_WithNegativeMaxVideos_ShouldFail()
    {
        var result = YoutubeCategory.Create(_id, _userId, "Test", -1, 0, _createdOnUtc);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldSucceed()
    {
        var category = YoutubeCategory.Create(_id, _userId, "Old", 5, 0, _createdOnUtc).Value;
        var utcNow = _createdOnUtc.AddHours(1);

        var result = category.UpdateName("New Name", utcNow);

        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("New Name");
        category.ModifiedOnUtc.Should().Be(utcNow);
    }

    [Fact]
    public void UpdateMaxVideosPerDay_WithNegativeValue_ShouldFail()
    {
        var category = YoutubeCategory.Create(_id, _userId, "Test", 5, 0, _createdOnUtc).Value;

        var result = category.UpdateMaxVideosPerDay(-1, _createdOnUtc.AddHours(1));

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateDisplayOrder_ShouldUpdateOrderAndModifiedOnUtc()
    {
        var category = YoutubeCategory.Create(_id, _userId, "Test", 5, 0, _createdOnUtc).Value;
        var utcNow = _createdOnUtc.AddHours(1);

        category.UpdateDisplayOrder(3, utcNow);

        category.DisplayOrder.Should().Be(3);
        category.ModifiedOnUtc.Should().Be(utcNow);
    }
}
