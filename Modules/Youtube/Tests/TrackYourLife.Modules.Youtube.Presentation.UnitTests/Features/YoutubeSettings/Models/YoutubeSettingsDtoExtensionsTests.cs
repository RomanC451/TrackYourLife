using System.Collections.Generic;
using FluentAssertions;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeSettings.Models;

public class YoutubeSettingsDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithPolicyAndSettings_ShouldMapCorrectly()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var settings = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            userId,
            SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 7,
            LastSettingsChangeUtc: utc.AddDays(-1),
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null,
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );
        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(catId, userId, "Work", MaxVideosPerDay: 3, DisplayOrder: 1, CreatedOnUtc: utc, ModifiedOnUtc: null),
        };
        var policy = new YoutubePolicyReadModel(settings, categories);

        var dto = policy.ToDto();

        dto.Categories.Should().HaveCount(1);
        dto.Categories[0].Name.Should().Be("Work");
        dto.Categories[0].MaxVideosPerDay.Should().Be(3);
        dto.Categories[0].SubscribedChannelCount.Should().Be(0);
        dto.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
        dto.DaysBetweenChanges.Should().Be(7);
    }

    [Fact]
    public void ToDto_WithNullSettings_ShouldMapCategoriesOnly()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(catId, userId, "A", MaxVideosPerDay: 0, DisplayOrder: 0, CreatedOnUtc: utc, ModifiedOnUtc: null),
        };
        var policy = new YoutubePolicyReadModel(null, categories);

        var dto = policy.ToDto();

        dto.SettingsChangeFrequency.Should().BeNull();
        dto.Categories.Should().HaveCount(1);
        dto.Categories[0].SubscribedChannelCount.Should().Be(0);
    }

    [Fact]
    public void ToDto_WithChannelCounts_ShouldMapSubscribedChannelCount()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(catId, userId, "A", MaxVideosPerDay: 1, DisplayOrder: 0, CreatedOnUtc: utc, ModifiedOnUtc: null),
        };
        var counts = new Dictionary<YoutubeCategoryId, int> { [catId] = 7 };
        var policy = new YoutubePolicyReadModel(null, categories, counts);

        var dto = policy.ToDto();

        dto.Categories[0].SubscribedChannelCount.Should().Be(7);
    }
}
