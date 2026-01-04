using FluentAssertions;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeSettings.Models;

public class YoutubeSettingsDtoExtensionsTests
{
    [Fact]
    public void ToDto_WithYoutubeSetting_ShouldMapCorrectly()
    {
        // Arrange
        var id = YoutubeSettingsId.NewId();
        var userId = UserId.NewId();
        var createdOnUtc = DateTime.UtcNow;
        var lastSettingsChangeUtc = DateTime.UtcNow.AddDays(-1);

        var settings = YoutubeSetting
            .Create(
                id,
                userId,
                maxEntertainmentVideosPerDay: 5,
                SettingsChangeFrequency.OnceEveryFewDays,
                daysBetweenChanges: 7,
                lastSettingsChangeUtc,
                specificDayOfWeek: null,
                specificDayOfMonth: null,
                createdOnUtc
            )
            .Value;

        // Act
        var dto = settings.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.MaxEntertainmentVideosPerDay.Should().Be(5);
        dto.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
        dto.DaysBetweenChanges.Should().Be(7);
        dto.LastSettingsChangeUtc.Should().Be(lastSettingsChangeUtc);
        dto.SpecificDayOfWeek.Should().BeNull();
        dto.SpecificDayOfMonth.Should().BeNull();
    }

    [Fact]
    public void ToDto_WithYoutubeSettingReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var id = YoutubeSettingsId.NewId();
        var userId = UserId.NewId();
        var createdOnUtc = DateTime.UtcNow;
        var lastSettingsChangeUtc = DateTime.UtcNow.AddDays(-1);
        var modifiedOnUtc = DateTime.UtcNow;

        var readModel = new YoutubeSettingReadModel(
            id,
            userId,
            MaxEntertainmentVideosPerDay: 10,
            SettingsChangeFrequency.SpecificDayOfWeek,
            DaysBetweenChanges: null,
            lastSettingsChangeUtc,
            SpecificDayOfWeek: DayOfWeek.Monday,
            SpecificDayOfMonth: null,
            createdOnUtc,
            modifiedOnUtc
        );

        // Act
        var dto = readModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.MaxEntertainmentVideosPerDay.Should().Be(10);
        dto.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.SpecificDayOfWeek);
        dto.DaysBetweenChanges.Should().BeNull();
        dto.LastSettingsChangeUtc.Should().Be(lastSettingsChangeUtc);
        dto.SpecificDayOfWeek.Should().Be(DayOfWeek.Monday);
        dto.SpecificDayOfMonth.Should().BeNull();
    }

    [Fact]
    public void ToDto_WithSpecificDayOfMonth_ShouldMapCorrectly()
    {
        // Arrange
        var id = YoutubeSettingsId.NewId();
        var userId = UserId.NewId();
        var createdOnUtc = DateTime.UtcNow;

        var readModel = new YoutubeSettingReadModel(
            id,
            userId,
            MaxEntertainmentVideosPerDay: 3,
            SettingsChangeFrequency.SpecificDayOfMonth,
            DaysBetweenChanges: null,
            LastSettingsChangeUtc: null,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: 15,
            createdOnUtc,
            ModifiedOnUtc: null
        );

        // Act
        var dto = readModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.MaxEntertainmentVideosPerDay.Should().Be(3);
        dto.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.SpecificDayOfMonth);
        dto.SpecificDayOfMonth.Should().Be(15);
        dto.SpecificDayOfWeek.Should().BeNull();
    }
}
