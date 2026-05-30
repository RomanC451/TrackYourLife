using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetHomeRecommendation;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Queries.GetHomeRecommendation;

public sealed class GetHomeRecommendationQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly GetHomeRecommendationQueryHandler _handler;

    public GetHomeRecommendationQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();

        _handler = new GetHomeRecommendationQueryHandler(
            _userIdentifierProvider,
            _youtubeChannelsQuery,
            _youtubeApiService,
            _watchedVideosRepository
        );
    }

    [Fact]
    public async Task Handle_WhenNoFavorites_ReturnsNull()
    {
        var userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetFavoritesByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<YoutubeChannelReadModel>());

        var result = await _handler.Handle(new GetHomeRecommendationQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
        await _youtubeApiService
            .DidNotReceive()
            .GetVideosFromChannelsAsync(
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenFavoritesExist_ReturnsRecommendation()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var favorites = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                catId,
                "Cat",
                true,
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
            .GetFavoritesByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(favorites);
        _youtubeApiService
            .GetVideosFromChannelsAsync(
                Arg.Is<IEnumerable<string>>(ids => ids.Single() == "channel-1"),
                2,
                Arg.Any<CancellationToken>()
            )
            .Returns(Result.Success<IEnumerable<YoutubeVideoPreview>>(videos));
        _watchedVideosRepository
            .GetByUserIdAndVideoIdsAsync(userId, Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<WatchedVideo>());

        var result = await _handler.Handle(new GetHomeRecommendationQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.VideoId.Should().Be("video-1");
    }

    [Fact]
    public async Task Handle_WhenApiFails_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var favorites = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                null,
                catId,
                "Cat",
                true,
                DateTime.UtcNow,
                null
            ),
        };
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetFavoritesByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(favorites);
        _youtubeApiService
            .GetVideosFromChannelsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IEnumerable<YoutubeVideoPreview>>(error));

        var result = await _handler.Handle(new GetHomeRecommendationQuery(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
