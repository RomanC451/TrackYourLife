using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.SearchYoutubeChannels;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Queries.SearchYoutubeChannels;

public sealed class SearchYoutubeChannelsQueryHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly SearchYoutubeChannelsQueryHandler _handler;

    public SearchYoutubeChannelsQueryHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();

        _handler = new SearchYoutubeChannelsQueryHandler(
            _youtubeApiService,
            _userIdentifierProvider,
            _youtubeChannelsQuery
        );
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
                1000,
                false
            )
        };

        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        var subscribedChannel = new YoutubeChannelReadModel(
            YoutubeChannelId.NewId(),
            userId,
            "channel-1",
            "Channel 1",
            "thumbnail",
            VideoCategory.Educational,
            DateTime.UtcNow,
            null
        );

        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new[] { subscribedChannel });

        _youtubeApiService
            .SearchChannelsAsync(query.Query, query.MaxResults, Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeChannelSearchResult>>(channels));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().AlreadySubscribed.Should().BeTrue();
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
