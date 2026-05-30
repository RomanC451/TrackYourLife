using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Commands;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class YoutubeChannelsTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    private async Task<YoutubeCategoryId> SeedEntertainmentCategoryAsync()
    {
        var catId = YoutubeCategoryId.NewId();
        var cat = YoutubeCategory
            .Create(
                catId,
                _user.Id,
                YoutubeCategoryDefaults.EntertainmentName,
                YoutubeCategoryDefaults.EntertainmentMaxVideosPerDay,
                YoutubeCategoryDefaults.EntertainmentDisplayOrder,
                DateTime.UtcNow
            )
            .Value;
        await _youtubeWriteDbContext.YoutubeCategories.AddAsync(cat);
        await _youtubeWriteDbContext.SaveChangesAsync();
        return catId;
    }

    private static YoutubeChannel CreateChannel(
        UserId userId,
        YoutubeCategoryId catId,
        string youtubeChannelId,
        string name,
        bool isFavorite
    )
    {
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                youtubeChannelId,
                name,
                null,
                catId,
                YoutubeCategoryDefaults.EntertainmentName,
                DateTime.UtcNow
            )
            .Value;

        if (isFavorite)
        {
            channel.SetFavorite(true, DateTime.UtcNow);
        }

        return channel;
    }

    [Fact]
    public async Task GetChannelsByCategory_WithNoChannels_ShouldReturnEmptyList()
    {
        var catId = await SeedEntertainmentCategoryAsync();

        var response = await _client.GetAsync($"/api/channels?youtubeCategoryId={catId.Value}");

        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChannelsByCategory_WithChannels_ShouldReturnChannels()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                youtubeCategoryId: catId,
                categoryName: YoutubeCategoryDefaults.EntertainmentName,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/channels?youtubeCategoryId={catId.Value}");

        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test Channel");
        result[0].YoutubeCategoryId.Should().Be(catId.Value);
        result[0].CategoryName.Should().Be(YoutubeCategoryDefaults.EntertainmentName);
    }

    [Fact]
    public async Task AddChannelToCategory_WithValidData_ShouldReturnCreated()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var validChannelId = "UC_x5XG1OV2P6uZZ5FSM9Ttw";
        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: validChannelId,
            YoutubeCategoryId: catId.Value
        );

        var response = await _client.PostAsJsonAsync("/api/channels", request);

        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        var channel = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.YoutubeChannelId == validChannelId);
        channel.Should().NotBeNull();
        channel!.YoutubeCategoryId.Should().Be(catId);
    }

    [Fact]
    public async Task AddChannelToCategory_WithDuplicateChannel_ShouldReturnBadRequest()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                youtubeCategoryId: catId,
                categoryName: YoutubeCategoryDefaults.EntertainmentName,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var eduId = YoutubeCategoryId.NewId();
        var edu = YoutubeCategory
            .Create(
                eduId,
                _user.Id,
                YoutubeCategoryDefaults.EducationalName,
                YoutubeCategoryDefaults.EducationalMaxVideosPerDay,
                YoutubeCategoryDefaults.EducationalDisplayOrder,
                DateTime.UtcNow
            )
            .Value;
        await _youtubeWriteDbContext.YoutubeCategories.AddAsync(edu);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var request = new AddChannelToCategoryRequest(
            YoutubeChannelId: "UCtest123",
            YoutubeCategoryId: eduId.Value
        );

        var response = await _client.PostAsJsonAsync("/api/channels", request);

        var error = await response.ShouldHaveStatusCodeAndContent<ProblemDetails>(HttpStatusCode.BadRequest);
        error!.Type.Should().Contain("AlreadyExists");
    }

    [Fact]
    public async Task RemoveChannel_WithValidId_ShouldReturnNoContent()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCtest123",
                name: "Test Channel",
                thumbnailUrl: null,
                youtubeCategoryId: catId,
                categoryName: YoutubeCategoryDefaults.EntertainmentName,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/channels/{channel.YoutubeChannelId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var deletedChannel = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == channel.Id);
        deletedChannel.Should().BeNull();
    }

    [Fact]
    public async Task RemoveChannel_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await _client.DeleteAsync("/api/channels/UC_DOES_NOT_EXIST_1234567890");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MoveChannelToCategory_WithValidData_ShouldMoveChannel()
    {
        var sourceCatId = await SeedEntertainmentCategoryAsync();
        var targetCatId = YoutubeCategoryId.NewId();
        var targetCat = YoutubeCategory
            .Create(
                targetCatId,
                _user.Id,
                YoutubeCategoryDefaults.EducationalName,
                YoutubeCategoryDefaults.EducationalMaxVideosPerDay,
                YoutubeCategoryDefaults.EducationalDisplayOrder,
                DateTime.UtcNow
            )
            .Value;
        await _youtubeWriteDbContext.YoutubeCategories.AddAsync(targetCat);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                _user.Id,
                youtubeChannelId: "UCmove123",
                name: "Move Me",
                thumbnailUrl: null,
                youtubeCategoryId: sourceCatId,
                categoryName: YoutubeCategoryDefaults.EntertainmentName,
                createdOnUtc: DateTime.UtcNow
            )
            .Value;

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var request = new MoveChannelToCategoryRequest(targetCatId.Value);
        var response = await _client.PatchAsync(
            $"/api/channels/{channel.YoutubeChannelId}/category",
            JsonContent.Create(request)
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var moved = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstAsync(c => c.YoutubeChannelId == channel.YoutubeChannelId);
        moved.YoutubeCategoryId.Should().Be(targetCatId);
        moved.CategoryName.Should().Be(YoutubeCategoryDefaults.EducationalName);
    }

    [Fact]
    public async Task SetChannelFavorite_WhenToggling_ShouldPersistFavoriteState()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var channel = CreateChannel(_user.Id, catId, "UCfav123", "Favorite Channel", isFavorite: false);

        await _youtubeWriteDbContext.YoutubeChannels.AddAsync(channel);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var favoriteResponse = await _client.PatchAsync(
            $"/api/channels/{channel.YoutubeChannelId}/favorite",
            JsonContent.Create(new SetChannelFavoriteRequest(true))
        );
        await favoriteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var favorited = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstAsync(c => c.YoutubeChannelId == channel.YoutubeChannelId);
        favorited.IsFavorite.Should().BeTrue();

        var listResponse = await _client.GetAsync("/api/channels?favoritesOnly=true");
        var favorites = await listResponse.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        favorites.Should().ContainSingle(c => c.YoutubeChannelId == channel.YoutubeChannelId && c.IsFavorite);

        var unfavoriteResponse = await _client.PatchAsync(
            $"/api/channels/{channel.YoutubeChannelId}/favorite",
            JsonContent.Create(new SetChannelFavoriteRequest(false))
        );
        await unfavoriteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var unfavorited = await _youtubeWriteDbContext
            .YoutubeChannels.AsNoTracking()
            .FirstAsync(c => c.YoutubeChannelId == channel.YoutubeChannelId);
        unfavorited.IsFavorite.Should().BeFalse();
    }

    [Fact]
    public async Task GetChannelsByCategory_WithFavoritesOnly_ShouldReturnOnlyFavoriteChannels()
    {
        var catId = await SeedEntertainmentCategoryAsync();
        var favorite = CreateChannel(_user.Id, catId, "UCfav456", "Favorite", isFavorite: true);
        var regular = CreateChannel(_user.Id, catId, "UCreg789", "Regular", isFavorite: false);

        await _youtubeWriteDbContext.YoutubeChannels.AddRangeAsync(favorite, regular);
        await _youtubeWriteDbContext.SaveChangesAsync();

        var response = await _client.GetAsync("/api/channels?favoritesOnly=true");

        var result = await response.ShouldHaveStatusCodeAndContent<List<YoutubeChannelDto>>(
            HttpStatusCode.OK
        );
        result.Should().HaveCount(1);
        result[0].YoutubeChannelId.Should().Be("UCfav456");
        result[0].IsFavorite.Should().BeTrue();
    }

    [Fact]
    public async Task GetChannelsByCategory_WithFavoritesOnlyAndCategory_ShouldReturnBadRequest()
    {
        var catId = await SeedEntertainmentCategoryAsync();

        var response = await _client.GetAsync(
            $"/api/channels?favoritesOnly=true&youtubeCategoryId={catId.Value}"
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SetChannelFavorite_WithUnknownChannel_ShouldReturnNotFound()
    {
        var response = await _client.PatchAsync(
            "/api/channels/UC_DOES_NOT_EXIST_1234567890/favorite",
            JsonContent.Create(new SetChannelFavoriteRequest(true))
        );

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
