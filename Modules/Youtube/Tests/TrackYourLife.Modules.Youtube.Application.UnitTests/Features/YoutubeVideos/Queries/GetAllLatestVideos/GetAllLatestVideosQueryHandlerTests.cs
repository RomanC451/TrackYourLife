using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetAllLatestVideos;

public sealed class GetAllLatestVideosQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly GetAllLatestVideosQueryHandler _handler;

    public GetAllLatestVideosQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();

        _handler = new GetAllLatestVideosQueryHandler(
            _userIdentifierProvider,
            _youtubeChannelsQuery,
            _youtubeApiService
        );
    }

    [Fact]
    public async Task Handle_WhenNoChannels_ReturnsEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<YoutubeChannelReadModel>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _youtubeApiService
            .DidNotReceive()
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategorySpecified_FiltersChannelsByCategory()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery(Category: VideoCategory.Educational);
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                VideoCategory.Educational,
                DateTime.UtcNow,
                null
            )
        };
        var videos = new List<YoutubeVideoPreview>
        {
            new(
                "video-1",
                "Video 1",
                "thumbnail",
                "Channel 1",
                "channel-1",
                DateTime.UtcNow,
                "PT10M",
                1000
            )
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAndCategoryAsync(userId, VideoCategory.Educational, Arg.Any<CancellationToken>())
            .Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeChannelsQuery
            .Received(1)
            .GetByUserIdAndCategoryAsync(userId, VideoCategory.Educational, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVideosRetrieved_SortsByPublishedDateDescending()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                VideoCategory.Educational,
                DateTime.UtcNow,
                null
            )
        };
        var now = DateTime.UtcNow;
        var videos = new List<YoutubeVideoPreview>
        {
            new("video-1", "Video 1", "thumbnail", "Channel 1", "channel-1", now.AddDays(-2), "PT10M", 1000),
            new("video-2", "Video 2", "thumbnail", "Channel 1", "channel-1", now, "PT10M", 1000),
            new("video-3", "Video 3", "thumbnail", "Channel 1", "channel-1", now.AddDays(-1), "PT10M", 1000)
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var sortedVideos = result.Value.ToList();
        sortedVideos[0].VideoId.Should().Be("video-2");
        sortedVideos[1].VideoId.Should().Be("video-3");
        sortedVideos[2].VideoId.Should().Be("video-1");
    }

    [Fact]
    public async Task Handle_WhenApiFails_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                VideoCategory.Educational,
                DateTime.UtcNow,
                null
            )
        };
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
