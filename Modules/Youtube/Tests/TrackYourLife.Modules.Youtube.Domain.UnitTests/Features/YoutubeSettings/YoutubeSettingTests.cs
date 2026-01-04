using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeSettings;

public class YoutubeSettingTests
{
    private readonly YoutubeSettingsId _id;
    private readonly UserId _userId;
    private readonly int _maxEntertainmentVideosPerDay;
    private readonly DateTime _createdOnUtc;

    public YoutubeSettingTests()
    {
        _id = YoutubeSettingsId.NewId();
        _userId = UserId.NewId();
        _maxEntertainmentVideosPerDay = 5;
        _createdOnUtc = DateTime.UtcNow;
    }

    [Fact]
    public void Create_WithValidParametersOnceEveryFewDays_ShouldCreateSetting()
    {
        // Arrange
        var frequency = SettingsChangeFrequency.OnceEveryFewDays;
        var daysBetweenChanges = 7;

        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            frequency,
            daysBetweenChanges,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.MaxEntertainmentVideosPerDay.Should().Be(_maxEntertainmentVideosPerDay);
        result.Value.SettingsChangeFrequency.Should().Be(frequency);
        result.Value.DaysBetweenChanges.Should().Be(daysBetweenChanges);
        result.Value.SpecificDayOfWeek.Should().BeNull();
        result.Value.SpecificDayOfMonth.Should().BeNull();
        result.Value.CreatedOnUtc.Should().Be(_createdOnUtc);
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidParametersSpecificDayOfWeek_ShouldCreateSetting()
    {
        // Arrange
        var frequency = SettingsChangeFrequency.SpecificDayOfWeek;
        var dayOfWeek = DayOfWeek.Monday;

        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            frequency,
            null,
            null,
            dayOfWeek,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SettingsChangeFrequency.Should().Be(frequency);
        result.Value.SpecificDayOfWeek.Should().Be(dayOfWeek);
        result.Value.DaysBetweenChanges.Should().BeNull();
        result.Value.SpecificDayOfMonth.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidParametersSpecificDayOfMonth_ShouldCreateSetting()
    {
        // Arrange
        var frequency = SettingsChangeFrequency.SpecificDayOfMonth;
        var dayOfMonth = 15;

        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            frequency,
            null,
            null,
            null,
            dayOfMonth,
            _createdOnUtc
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SettingsChangeFrequency.Should().Be(frequency);
        result.Value.SpecificDayOfMonth.Should().Be(dayOfMonth);
        result.Value.DaysBetweenChanges.Should().BeNull();
        result.Value.SpecificDayOfWeek.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = YoutubeSettingsId.Empty;

        // Act
        var result = YoutubeSetting.Create(
            emptyId,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            7,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeSetting)}.{nameof(YoutubeSetting.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var emptyUserId = UserId.Empty;

        // Act
        var result = YoutubeSetting.Create(
            _id,
            emptyUserId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            7,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeSetting)}.{nameof(YoutubeSetting.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithDefaultCreatedOnUtc_ShouldFail()
    {
        // Arrange
        var defaultDateTime = default(DateTime);

        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            7,
            null,
            null,
            null,
            defaultDateTime
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(YoutubeSetting)}.{nameof(YoutubeSetting.CreatedOnUtc).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithOnceEveryFewDaysAndNullDaysBetweenChanges_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            null,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDaysBetweenChanges");
    }

    [Fact]
    public void Create_WithOnceEveryFewDaysAndZeroDaysBetweenChanges_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            0,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDaysBetweenChanges");
    }

    [Fact]
    public void Create_WithSpecificDayOfWeekAndNullDayOfWeek_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfWeek,
            null,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.SpecificDayOfWeekRequired");
    }

    [Fact]
    public void Create_WithSpecificDayOfMonthAndNullDayOfMonth_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfMonth,
            null,
            null,
            null,
            null,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDayOfMonth");
    }

    [Fact]
    public void Create_WithSpecificDayOfMonthAndZero_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfMonth,
            null,
            null,
            null,
            0,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDayOfMonth");
    }

    [Fact]
    public void Create_WithSpecificDayOfMonthAndGreaterThan31_ShouldFail()
    {
        // Act
        var result = YoutubeSetting.Create(
            _id,
            _userId,
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfMonth,
            null,
            null,
            null,
            32,
            _createdOnUtc
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDayOfMonth");
    }

    [Fact]
    public void UpdateSettings_WithValidParameters_ShouldUpdateSettings()
    {
        // Arrange
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.OnceEveryFewDays,
                7,
                null,
                null,
                null,
                _createdOnUtc
            )
            .Value;
        var newMaxVideos = 10;
        var newFrequency = SettingsChangeFrequency.SpecificDayOfWeek;
        var newDayOfWeek = DayOfWeek.Friday;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.UpdateSettings(
            newMaxVideos,
            newFrequency,
            null,
            newDayOfWeek,
            null,
            utcNow
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        setting.MaxEntertainmentVideosPerDay.Should().Be(newMaxVideos);
        setting.SettingsChangeFrequency.Should().Be(newFrequency);
        setting.SpecificDayOfWeek.Should().Be(newDayOfWeek);
        setting.LastSettingsChangeUtc.Should().Be(utcNow);
        setting.ModifiedOnUtc.Should().Be(utcNow);
    }

    [Fact]
    public void UpdateSettings_WithInvalidFrequencyConfiguration_ShouldFail()
    {
        // Arrange
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.OnceEveryFewDays,
                7,
                null,
                null,
                null,
                _createdOnUtc
            )
            .Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.UpdateSettings(
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.OnceEveryFewDays,
            null,
            null,
            null,
            utcNow
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.Settings.InvalidDaysBetweenChanges");
    }

    [Fact]
    public void CanChangeSettings_WhenNeverChangedBefore_ShouldReturnSuccess()
    {
        // Arrange
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.OnceEveryFewDays,
                7,
                null,
                null,
                null,
                _createdOnUtc
            )
            .Value;

        // Act
        var result = setting.CanChangeSettings(DateTime.UtcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanChangeSettings_WithOnceEveryFewDaysAndEnoughDaysPassed_ShouldReturnSuccess()
    {
        // Arrange
        var lastChange = DateTime.UtcNow.AddDays(-10);
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.OnceEveryFewDays,
                7,
                lastChange,
                null,
                null,
                _createdOnUtc
            )
            .Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.CanChangeSettings(utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanChangeSettings_WithOnceEveryFewDaysAndNotEnoughDaysPassed_ShouldFail()
    {
        // Arrange
        var lastChange = DateTime.UtcNow.AddDays(-5);
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.OnceEveryFewDays,
                7,
                lastChange,
                null,
                null,
                _createdOnUtc
            )
            .Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.CanChangeSettings(utcNow);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.SettingsChangeNotAllowed");
    }

    [Fact]
    public void CanChangeSettings_WithSpecificDayOfWeekAndCorrectDay_ShouldReturnSuccess()
    {
        // Arrange
        var dayOfWeek = DateTime.UtcNow.DayOfWeek;
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.SpecificDayOfWeek,
                null,
                null,
                dayOfWeek,
                null,
                _createdOnUtc
            )
            .Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.CanChangeSettings(utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanChangeSettings_WithSpecificDayOfWeekAndWrongDay_ShouldFail()
    {
        // Arrange - Use a fixed date to avoid timing issues
        var fixedDate = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc); // Monday
        var wrongDayOfWeek = DayOfWeek.Tuesday; // Different from Monday
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.SpecificDayOfWeek,
                null,
                null,
                wrongDayOfWeek,
                null,
                _createdOnUtc
            )
            .Value;
        // Set last change to yesterday to trigger the day check
        setting.UpdateSettings(
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfWeek,
            null,
            wrongDayOfWeek,
            null,
            fixedDate.AddDays(-1)
        );

        // Act
        var result = setting.CanChangeSettings(fixedDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.SettingsChangeNotAllowed");
    }

    [Fact]
    public void CanChangeSettings_WithSpecificDayOfMonthAndCorrectDay_ShouldReturnSuccess()
    {
        // Arrange
        var dayOfMonth = DateTime.UtcNow.Day;
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.SpecificDayOfMonth,
                null,
                null,
                null,
                dayOfMonth,
                _createdOnUtc
            )
            .Value;
        var utcNow = DateTime.UtcNow;

        // Act
        var result = setting.CanChangeSettings(utcNow);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void CanChangeSettings_WithSpecificDayOfMonthAndWrongDay_ShouldFail()
    {
        // Arrange - Use a fixed date to avoid timing issues
        var fixedDate = new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc); // Day 15
        var wrongDayOfMonth = 20; // Different from 15
        var setting = YoutubeSetting
            .Create(
                _id,
                _userId,
                _maxEntertainmentVideosPerDay,
                SettingsChangeFrequency.SpecificDayOfMonth,
                null,
                null,
                null,
                wrongDayOfMonth,
                _createdOnUtc
            )
            .Value;
        // Set last change to yesterday to trigger the day check
        setting.UpdateSettings(
            _maxEntertainmentVideosPerDay,
            SettingsChangeFrequency.SpecificDayOfMonth,
            null,
            null,
            wrongDayOfMonth,
            fixedDate.AddDays(-1)
        );

        // Act
        var result = setting.CanChangeSettings(fixedDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Youtube.SettingsChangeNotAllowed");
    }

}
