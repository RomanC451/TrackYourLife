using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeSettings;

public class YoutubeSettingTests
{
    private readonly YoutubeSettingsId _id = YoutubeSettingsId.NewId();
    private readonly UserId _userId = UserId.NewId();
    private readonly DateTime _createdOnUtc = DateTime.UtcNow;

    [Fact]
    public void Create_WithOnceEveryFewDays_ShouldSucceed()
    {
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            SettingsChangeFrequency.OnceEveryFewDays,
            daysBetweenChanges: 7,
            lastSettingsChangeUtc: null,
            specificDayOfWeek: null,
            specificDayOfMonth: null,
            _createdOnUtc
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
        result.Value.DaysBetweenChanges.Should().Be(7);
    }

    [Fact]
    public void Create_WithSpecificDayOfWeek_ShouldSucceed()
    {
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            SettingsChangeFrequency.SpecificDayOfWeek,
            null,
            null,
            DayOfWeek.Wednesday,
            null,
            _createdOnUtc
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.SpecificDayOfWeek.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void Create_WithOnceEveryFewDaysAndMissingDays_ShouldFail()
    {
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            SettingsChangeFrequency.OnceEveryFewDays,
            null,
            null,
            null,
            null,
            _createdOnUtc
        );

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void CanChangeSettings_WhenNeverChanged_ShouldAllow()
    {
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                SettingsChangeFrequency.OnceEveryFewDays,
                1,
                null,
                null,
                null,
                _createdOnUtc
            )
            .Value;

        setting.CanChangeSettings(DateTime.UtcNow).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UpdateSettings_ShouldUpdateFrequencyFields()
    {
        var utc = new DateTime(2026, 5, 14, 12, 0, 0, DateTimeKind.Utc);
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                SettingsChangeFrequency.OnceEveryFewDays,
                1,
                utc.AddDays(-5),
                null,
                null,
                _createdOnUtc
            )
            .Value;

        var update = setting.UpdateSettings(
            SettingsChangeFrequency.SpecificDayOfMonth,
            null,
            null,
            20,
            utc
        );

        update.IsSuccess.Should().BeTrue();
        setting.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.SpecificDayOfMonth);
        setting.SpecificDayOfMonth.Should().Be(20);
    }
}
