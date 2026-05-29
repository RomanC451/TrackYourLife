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
        result.HasSettingsPassword.Should().BeFalse();
    }

    [Fact]
    public async Task GetYoutubeSettings_WithExistingPassword_ShouldReturnHasSettingsPassword()
    {
        var settings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                _user.Id,
                settingsPasswordHash: "salt;hash",
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeSettings.AddAsync(settings);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.GetAsync("/api/settings");

        var result = await response.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(HttpStatusCode.OK);
        result!.HasSettingsPassword.Should().BeTrue();
        result.Categories.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SetAndVerifyYoutubeSettingsPassword_ShouldSucceed()
    {
        var setResponse = await _client.PutAsJsonAsync(
            "/api/settings/password",
            new SetYoutubeSettingsPasswordRequest(null, "ValidPass1!")
        );

        await setResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var verifyResponse = await _client.PostAsJsonAsync(
            "/api/settings/password/verify",
            new VerifyYoutubeSettingsPasswordRequest("ValidPass1!")
        );

        await verifyResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings.Should().NotBeNull();
        settings!.HasPassword.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyYoutubeSettingsPassword_WithWrongPassword_ShouldReturnUnauthorized()
    {
        var settings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                _user.Id,
                "salt;hash",
                DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeSettings.AddAsync(settings);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.PostAsJsonAsync(
            "/api/settings/password/verify",
            new VerifyYoutubeSettingsPasswordRequest("WrongPass1!")
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ResetYoutubeSettingsPasswordViaEmail_WhenLockEnabled_ShouldReturnNoContent()
    {
        var setResponse = await _client.PutAsJsonAsync(
            "/api/settings/password",
            new SetYoutubeSettingsPasswordRequest(null, "ValidPass1!")
        );
        await setResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var resetResponse = await _client.PostAsync(
            "/api/settings/password/reset-email",
            null
        );

        await resetResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var settings = await _youtubeWriteDbContext
            .YoutubeSettings.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == _user.Id);
        settings.Should().NotBeNull();
        settings!.HasPassword.Should().BeTrue();
        settings.SettingsPasswordResetEmailSentAtUtc.Should().NotBeNull();
    }
}
