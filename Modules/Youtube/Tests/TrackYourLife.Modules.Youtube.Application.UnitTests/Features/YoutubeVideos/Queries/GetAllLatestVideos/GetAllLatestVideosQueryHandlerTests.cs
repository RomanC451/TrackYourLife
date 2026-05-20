using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetAllLatestVideosQueryHandler _handler;

    public GetAllLatestVideosQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();

        _handler = new GetAllLatestVideosQueryHandler(
            _userIdentifierProvider,
            _youtubeChannelsQuery,
            _youtubeApiService,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenNoChannels_ReturnsEmptyList()
    {
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<YoutubeChannelReadModel>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        await _youtubeApiService
            .DidNotReceive()
            .GetVideosFromChannelsAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenCategorySpecified_UsesCategoryQuery()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var query = new GetAllLatestVideosQuery(MaxResultsPerChannel: 5, YoutubeCategoryId: catId);
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                catId,
                "Cat",
                DateTime.UtcNow,
                null
            ),
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
                1000,
                IsWatched: false
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeCategoryIdAsync(userId, catId, Arg.Any<CancellationToken>())
            .Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(
                Arg.Is<IEnumerable<string>>(ids => ids.Single() == "channel-1"),
                5,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(userId, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<WatchedVideo>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeChannelsQuery
            .DidNotReceive()
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApiFails_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var query = new GetAllLatestVideosQuery();
        var catId = YoutubeCategoryId.NewId();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "c1",
                "N",
                null,
                catId,
                "Cat",
                DateTime.UtcNow,
                null
            ),
        };
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error));

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_MarksWatchedVideos()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var query = new GetAllLatestVideosQuery();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "c1",
                "N",
                null,
                catId,
                "Cat",
                DateTime.UtcNow,
                null
            ),
        };
        var videos = new List<YoutubeVideoPreview>
        {
            new("v1", "T", "th", "N", "c1", DateTime.UtcNow, "PT1M", 1, false),
        };
        var watched = WatchedVideo
            .Create(WatchedVideoId.NewId(), userId, "v1", "c1", DateTime.UtcNow)
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(channels);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(userId, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(new List<WatchedVideo> { watched });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Single().IsWatched.Should().BeTrue();
    }
}
