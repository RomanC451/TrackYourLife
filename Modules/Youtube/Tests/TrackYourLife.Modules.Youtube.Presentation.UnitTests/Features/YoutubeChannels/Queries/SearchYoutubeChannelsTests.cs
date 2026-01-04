using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Queries;

public class SearchYoutubeChannelsTests
{
    private readonly ISender _sender;
    private readonly SearchYoutubeChannels _endpoint;

    public SearchYoutubeChannelsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new SearchYoutubeChannels(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithResults()
    {
        // Arrange
        var searchResults = new List<YoutubeChannelSearchResult>
        {
            new(
                ChannelId: "UCtest123",
                Name: "Test Channel 1",
                Description: "Test Description 1",
                ThumbnailUrl: "https://example.com/thumb1.jpg",
                SubscriberCount: 1000
            ),
            new(
                ChannelId: "UCtest456",
                Name: "Test Channel 2",
                Description: "Test Description 2",
                ThumbnailUrl: string.Empty,
                SubscriberCount: 2000
            ),
        };

        _sender
            .Send(Arg.Any<SearchYoutubeChannelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeChannelSearchResult>>(searchResults)
                )
            );

        var request = new SearchYoutubeChannelsRequest { Query = "test", MaxResults = 10 };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result
            .Should()
            .BeOfType<Ok<IEnumerable<YoutubeChannelSearchResult>>>()
            .Subject;
        okResult.Value.Should().NotBeNull();
        var results = okResult.Value!.ToList();
        results.Should().HaveCount(2);
        results[0].ChannelId.Should().Be("UCtest123");
        results[0].Name.Should().Be("Test Channel 1");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<SearchYoutubeChannelsQuery>(q => q.Query == "test" && q.MaxResults == 10),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenMaxResultsNotSpecified_ShouldUseDefault()
    {
        // Arrange
        _sender
            .Send(Arg.Any<SearchYoutubeChannelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeChannelSearchResult>>(
                        new List<YoutubeChannelSearchResult>()
                    )
                )
            );

        var request = new SearchYoutubeChannelsRequest { Query = "test", MaxResults = 10 };

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<SearchYoutubeChannelsQuery>(q => q.Query == "test" && q.MaxResults == 10),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<SearchYoutubeChannelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(Result.Failure<IEnumerable<YoutubeChannelSearchResult>>(error))
            );

        var request = new SearchYoutubeChannelsRequest { Query = "test", MaxResults = 5 };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
