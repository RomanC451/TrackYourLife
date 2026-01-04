using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

public sealed class SearchYoutubeChannelsQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly SearchYoutubeChannelsQueryHandler _handler;

    public SearchYoutubeChannelsQueryHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _handler = new SearchYoutubeChannelsQueryHandler(_youtubeApiService);
    }

    [Fact]
    public async Task Handle_WhenSearchSucceeds_ReturnsChannels()
    {
        // Arrange
        var query = new SearchYoutubeChannelsQuery("test query", MaxResults: 10);
        var channels = new List<YoutubeChannelSearchResult>
        {
            new(
                "channel-1",
                "Channel 1",
                "Description",
                "thumbnail",
                1000
            )
        };

        _youtubeApiService
            .SearchChannelsAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeChannelSearchResult>>(channels));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeApiService
            .Received(1)
            .SearchChannelsAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSearchFails_ReturnsFailure()
    {
        // Arrange
        var query = new SearchYoutubeChannelsQuery("test query", MaxResults: 10);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _youtubeApiService
            .SearchChannelsAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeChannelSearchResult>>(error));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
