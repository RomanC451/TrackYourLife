using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;
using TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Features;

[Collection("Youtube Integration Tests")]
public class LibraryPlaylistsTests(YoutubeFunctionalTestWebAppFactory factory)
    : YoutubeBaseIntegrationTest(factory)
{
    private sealed record JsonIdResponse(Guid Id);
    [Fact]
    public async Task GetPlaylists_WhenEmpty_ShouldReturnEmptyList()
    {
        var response = await _client.GetAsync("/api/library/playlists");

        var result = await response.ShouldHaveStatusCodeAndContent<
            List<YoutubePlaylistWithVideoPreviews>
        >(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePlaylist_ThenGetList_ShouldReturnPlaylist()
    {
        var createRequest = new CreatePlaylistRequest("My list");

        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            createRequest
        );

        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        idResponse.Should().NotBeNull();
        idResponse!.Id.Should().NotBeEmpty();

        var listResponse = await _client.GetAsync("/api/library/playlists");
        var list = await listResponse.ShouldHaveStatusCodeAndContent<
            List<YoutubePlaylistWithVideoPreviews>
        >(HttpStatusCode.OK);
        list.Should().HaveCount(1);
        list![0].Name.Should().Be("My list");
        list[0].VideoPreviews.Should().BeEmpty();
    }

    [Fact]
    public async Task GetPlaylistById_WhenNotFound_ShouldReturnNotFound()
    {
        var missingId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/library/playlists/{missingId}");

        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddVideoToPlaylist_ThenGetDetail_ShouldIncludeVideo()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            new CreatePlaylistRequest("With videos")
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        var playlistId = idResponse!.Id;

        var addResponse = await _client.PostAsJsonAsync(
            $"/api/library/playlists/{playlistId}/videos",
            new AddVideoToPlaylistRequest("dQw4w9WgXcQ")
        );
        await addResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var detailResponse = await _client.GetAsync($"/api/library/playlists/{playlistId}");
        var detail = await detailResponse.ShouldHaveStatusCodeAndContent<YoutubePlaylistDetailDto>(
            HttpStatusCode.OK
        );
        detail!.Videos.Should().HaveCount(1);
        detail.Videos[0].YoutubeId.Should().Be("dQw4w9WgXcQ");
    }

    [Fact]
    public async Task AddVideoToPlaylist_WhenDuplicate_ShouldReturnBadRequest()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            new CreatePlaylistRequest("Dup test")
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        var playlistId = idResponse!.Id;
        const string videoId = "abc12345678";

        await _client.PostAsJsonAsync(
            $"/api/library/playlists/{playlistId}/videos",
            new AddVideoToPlaylistRequest(videoId)
        );

        var dupResponse = await _client.PostAsJsonAsync(
            $"/api/library/playlists/{playlistId}/videos",
            new AddVideoToPlaylistRequest(videoId)
        );

        var error = await dupResponse.ShouldHaveStatusCodeAndContent<ProblemDetails>(
            HttpStatusCode.BadRequest
        );
        error!.Type.Should().Contain("VideoAlreadyInPlaylist");
    }

    [Fact]
    public async Task UpdatePlaylist_ThenGetDetail_ShouldReturnNewName()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            new CreatePlaylistRequest("Original")
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        var playlistId = idResponse!.Id;

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/library/playlists/{playlistId}",
            new UpdatePlaylistRequest("Renamed")
        );
        await updateResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var detailResponse = await _client.GetAsync($"/api/library/playlists/{playlistId}");
        var detail = await detailResponse.ShouldHaveStatusCodeAndContent<YoutubePlaylistDetailDto>(
            HttpStatusCode.OK
        );
        detail!.Name.Should().Be("Renamed");
    }

    [Fact]
    public async Task RemoveVideoFromPlaylist_ThenGetDetail_ShouldExcludeVideo()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            new CreatePlaylistRequest("Trim list")
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        var playlistId = idResponse!.Id;
        const string videoId = "removeMeVid12";

        await _client.PostAsJsonAsync(
            $"/api/library/playlists/{playlistId}/videos",
            new AddVideoToPlaylistRequest(videoId)
        );

        var removeResponse = await _client.DeleteAsync(
            $"/api/library/playlists/{playlistId}/videos/{videoId}"
        );
        await removeResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var detailResponse = await _client.GetAsync($"/api/library/playlists/{playlistId}");
        var detail = await detailResponse.ShouldHaveStatusCodeAndContent<YoutubePlaylistDetailDto>(
            HttpStatusCode.OK
        );
        detail!.Videos.Should().BeEmpty();
    }

    [Fact]
    public async Task DeletePlaylist_ThenGetById_ShouldReturnNotFound()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/library/playlists",
            new CreatePlaylistRequest("Gone soon")
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        var idResponse = await createResponse.Content.ReadFromJsonAsync<JsonIdResponse>();
        var playlistId = idResponse!.Id;

        var deleteResponse = await _client.DeleteAsync($"/api/library/playlists/{playlistId}");
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/library/playlists/{playlistId}");
        await getResponse.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }
}
