using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class YoutubeCategoriesTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    private static async Task<Guid> ReadCreatedIdAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return Guid.Parse(doc.RootElement.GetProperty("id").GetString()!);
    }

    [Fact]
    public async Task CreateYoutubeCategory_WithValidData_ShouldReturnCreated()
    {
        var request = new CreateYoutubeCategoryRequest("Custom Category", 3);

        var response = await _client.PostAsJsonAsync("/api/settings/categories", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Created);
        var createdId = await ReadCreatedIdAsync(response);

        var category = await _youtubeWriteDbContext
            .YoutubeCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == YoutubeCategoryId.Create(createdId));
        category.Should().NotBeNull();
        category!.Name.Should().Be("Custom Category");
        category.MaxVideosPerDay.Should().Be(3);
    }

    [Fact]
    public async Task UpdateYoutubeCategoryLimit_WithValidData_ShouldReturnNoContent()
    {
        var settingsResponse = await _client.GetAsync("/api/settings");
        var settings = await settingsResponse.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(
            HttpStatusCode.OK
        );
        var categoryId = settings!.Categories[0].Id;
        var previousMax = settings.Categories[0].MaxVideosPerDay;

        var request = new UpdateYoutubeCategoryLimitRequest(previousMax + 2);
        var response = await _client.PutAsJsonAsync(
            $"/api/settings/categories/{categoryId}/limit",
            request
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var category = await _youtubeWriteDbContext
            .YoutubeCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == YoutubeCategoryId.Create(categoryId));
        category.Should().NotBeNull();
        category!.MaxVideosPerDay.Should().Be(previousMax + 2);
    }

    [Fact]
    public async Task UpdateYoutubeCategoryMetadata_WithValidData_ShouldReturnNoContent()
    {
        var settingsResponse = await _client.GetAsync("/api/settings");
        var settings = await settingsResponse.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(
            HttpStatusCode.OK
        );
        var categoryId = settings!.Categories[0].Id;

        var request = new UpdateYoutubeCategoryMetadataRequest("Renamed Category", 5);
        var response = await _client.PutAsJsonAsync($"/api/settings/categories/{categoryId}", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var category = await _youtubeWriteDbContext
            .YoutubeCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == YoutubeCategoryId.Create(categoryId));
        category.Should().NotBeNull();
        category!.Name.Should().Be("Renamed Category");
        category.DisplayOrder.Should().Be(5);
    }

    [Fact]
    public async Task DeleteYoutubeCategory_WithNoChannels_ShouldReturnNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/settings/categories",
            new CreateYoutubeCategoryRequest("To Delete", 1)
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var createdId = await ReadCreatedIdAsync(createResponse);

        var response = await _client.DeleteAsync($"/api/settings/categories/{createdId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var deleted = await _youtubeWriteDbContext
            .YoutubeCategories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == YoutubeCategoryId.Create(createdId));
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteYoutubeCategory_WithChannelsWithoutConfirmation_ShouldReturnBadRequest()
    {
        var settingsResponse = await _client.GetAsync("/api/settings");
        var settings = await settingsResponse.ShouldHaveStatusCodeAndContent<YoutubeSettingsDto>(
            HttpStatusCode.OK
        );
        var categoryId = settings!.Categories[0].Id;

        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCcat123",
                name: "Cat Channel",
                thumbnailUrl: null,
                youtubeCategoryId: YoutubeCategoryId.Create(categoryId),
                categoryName: settings.Categories[0].Name,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/settings/categories/{categoryId}");

        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(HttpStatusCode.BadRequest);
        error!.Type.Should().Contain("DeleteRequiresConfirmation");
    }
}
