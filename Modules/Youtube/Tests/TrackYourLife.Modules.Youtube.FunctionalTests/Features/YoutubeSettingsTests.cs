using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class YoutubeSettingsTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetYoutubeSettings_WithNoExistingSettings_ShouldReturnNull()
    {
        // Act
        var response = await _client.GetAsync("/api/settings");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            // Empty response means null
            return;
        }
        // If content exists, it should be null JSON
        var result = System.Text.Json.JsonSerializer.Deserialize<YoutubeSettingsDto>(content);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetYoutubeSettings_WithExistingSettings_ShouldReturnSettings()
    {
        // Arrange
        var settings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                _user.Id,
                maxEntertainmentVideosPerDay: 5,
                settingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
                daysBetweenChanges: 1,
                lastSettingsChangeUtc: DateTime.UtcNow,
                specificDayOfWeek: null,
                specificDayOfMonth: null,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeSettings.AddAsync(settings);
        await _youtubeWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/settings");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(
            HttpStatusCode.OK
        );
        result.Should().NotBeNull();
        result!.MaxEntertainmentVideosPerDay.Should().Be(5);
        result.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
    }

    [Fact]
    public async Task UpdateYoutubeSettings_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new UpdateYoutubeSettingsRequest(
            MaxEntertainmentVideosPerDay: 10,
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 2,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/settings", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Verify settings were created
        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings.Should().NotBeNull();
        settings!.MaxEntertainmentVideosPerDay.Should().Be(request.MaxEntertainmentVideosPerDay);
        settings.SettingsChangeFrequency.Should().Be(request.SettingsChangeFrequency);
    }

    [Fact]
    public async Task UpdateYoutubeSettings_WithSpecificDayOfWeek_ShouldReturnCreated()
    {
        // Arrange
        var request = new UpdateYoutubeSettingsRequest(
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency: SettingsChangeFrequency.SpecificDayOfWeek,
            DaysBetweenChanges: null,
            SpecificDayOfWeek: DayOfWeek.Monday,
            SpecificDayOfMonth: null
        );

        // Act
        var response = await _client.PutAsJsonAsync("/api/settings", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Verify settings were created
        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings.Should().NotBeNull();
        settings!.SpecificDayOfWeek.Should().Be(DayOfWeek.Monday);
    }
}
