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
    public async Task GetYoutubeSettings_ShouldReturnCategories()
    {
        var response = await _client.GetAsync("/api/settings");

        var result = await response.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.Categories.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetYoutubeSettings_WithExistingSettings_ShouldReturnSettings()
    {
        var settings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                _user.Id,
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

        var response = await _client.GetAsync("/api/settings");

        var result = await response.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(HttpStatusCode.OK);
        result!.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
        result.Categories.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateYoutubeSettings_WithValidData_ShouldReturnCreated()
    {
        var getResp = await _client.GetAsync("/api/settings");
        var current = await getResp.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(HttpStatusCode.OK);
        current!.Categories.Should().NotBeEmpty();

        var request = new UpdateYoutubeSettingsRequest(
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 2,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        var response = await _client.PutAsJsonAsync("/api/settings", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings.Should().NotBeNull();
        settings!.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);
    }

    [Fact]
    public async Task UpdateYoutubeSettings_WithSpecificDayOfWeek_ShouldReturnCreated()
    {
        var getResp = await _client.GetAsync("/api/settings");
        await getResp.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(HttpStatusCode.OK);

        var request = new UpdateYoutubeSettingsRequest(
            SettingsChangeFrequency: SettingsChangeFrequency.SpecificDayOfWeek,
            DaysBetweenChanges: null,
            SpecificDayOfWeek: DayOfWeek.Monday,
            SpecificDayOfMonth: null
        );

        var response = await _client.PutAsJsonAsync("/api/settings", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings!.SpecificDayOfWeek.Should().Be(DayOfWeek.Monday);
    }
}
