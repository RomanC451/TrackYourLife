using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeVideos.Commands.PlayVideo;

public sealed class PlayVideoCommandHandlerTests
{
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IWatchedVideosRepository _watchedVideosRepository;
    private readonly IDailyCategoryWatchCountersRepository _dailyCategoryWatchCountersRepository;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeCategoriesQuery _youtubeCategoriesQuery;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PlayVideoCommandHandler _handler;

    public PlayVideoCommandHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();
        _dailyCategoryWatchCountersRepository = Substitute.For<IDailyCategoryWatchCountersRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeCategoriesQuery = Substitute.For<IYoutubeCategoriesQuery>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new PlayVideoCommandHandler(
            _youtubeApiService,
            _userIdentifierProvider,
            _watchedVideosRepository,
            _dailyCategoryWatchCountersRepository,
            _youtubeChannelsQuery,
            _youtubeCategoriesQuery,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenVideoAlreadyWatched_ReturnsVideoDetails()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var command = new PlayVideoCommand(videoId);
        var watchedVideo = WatchedVideo
            .Create(WatchedVideoId.NewId(), userId, videoId, "channel-id", DateTime.UtcNow)
            .Value;
        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            "channel-id",
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns(watchedVideo);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(videoDetails);
        await _watchedVideosRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<WatchedVideo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVideoDetailsApiFails_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var command = new PlayVideoCommand(videoId);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _userIdentifierProvider.UserId.Returns(userId);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeVideoDetails>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_WhenCategoryDailyLimitReached_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new PlayVideoCommand(videoId);
        var utcNow = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(utcNow);

        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            channelId,
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        var counter = DailyCategoryWatchCounter
            .Create(DailyCategoryWatchCounterId.NewId(), userId, today, categoryId, 5)
            .Value;

        var channelReadModel = new YoutubeChannelReadModel(
            YoutubeChannelId.NewId(),
            userId,
            channelId,
            "Channel Name",
            "thumbnail-url",
            categoryId,
            "Cat",
            DateTime.UtcNow,
            null
        );

        var categoryReadModel = new YoutubeCategoryReadModel(
            categoryId,
            userId,
            "Cat",
            MaxVideosPerDay: 5,
            DisplayOrder: 0,
            CreatedOnUtc: utcNow,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns(channelReadModel);
        _youtubeCategoriesQuery
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(categoryReadModel);
        _dailyCategoryWatchCountersRepository
            .GetByUserIdDateAndCategoryAsync(userId, today, categoryId, Arg.Any<CancellationToken>())
            .Returns(counter);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.CategoryLimitReached);
    }

    [Fact]
    public async Task Handle_WhenNotSubscribedToChannel_DoesNotTouchCategoryCounters()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
        var command = new PlayVideoCommand(videoId);
        var utcNow = DateTime.UtcNow;

        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            channelId,
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannelReadModel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _watchedVideosRepository
            .Received(1)
            .AddAsync(Arg.Is<WatchedVideo>(w => w.VideoId == videoId), Arg.Any<CancellationToken>());
        await _dailyCategoryWatchCountersRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<DailyCategoryWatchCounter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSubscribedAndUnderLimit_IncrementsCategoryCounter()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new PlayVideoCommand(videoId);
        var utcNow = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(utcNow);

        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            channelId,
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        var channelReadModel = new YoutubeChannelReadModel(
            YoutubeChannelId.NewId(),
            userId,
            channelId,
            "Channel Name",
            "thumbnail-url",
            categoryId,
            "Cat",
            DateTime.UtcNow,
            null
        );

        var categoryReadModel = new YoutubeCategoryReadModel(
            categoryId,
            userId,
            "Cat",
            MaxVideosPerDay: 5,
            DisplayOrder: 0,
            CreatedOnUtc: utcNow,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns(channelReadModel);
        _youtubeCategoriesQuery
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(categoryReadModel);
        _dailyCategoryWatchCountersRepository
            .GetByUserIdDateAndCategoryAsync(userId, today, categoryId, Arg.Any<CancellationToken>())
            .Returns((DailyCategoryWatchCounter?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _dailyCategoryWatchCountersRepository
            .Received(1)
            .AddAsync(Arg.Any<DailyCategoryWatchCounter>(), Arg.Any<CancellationToken>());
        _dailyCategoryWatchCountersRepository.Received(1).Update(Arg.Any<DailyCategoryWatchCounter>());
    }

    [Fact]
    public async Task Handle_WhenMaxVideosPerDayIsZero_DoesNotUseCategoryCounterRepository()
    {
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new PlayVideoCommand(videoId);
        var utcNow = DateTime.UtcNow;

        var videoDetails = new YoutubeVideoDetails(
            videoId,
            "Test Video",
            "Description",
            "https://embed.url",
            "https://thumbnail.url",
            "Channel Name",
            channelId,
            DateTime.UtcNow,
            "PT10M",
            1000,
            100
        );

        var channelReadModel = new YoutubeChannelReadModel(
            YoutubeChannelId.NewId(),
            userId,
            channelId,
            "Channel Name",
            "thumbnail-url",
            categoryId,
            "Cat",
            DateTime.UtcNow,
            null
        );

        var categoryReadModel = new YoutubeCategoryReadModel(
            categoryId,
            userId,
            "Cat",
            MaxVideosPerDay: 0,
            DisplayOrder: 0,
            CreatedOnUtc: utcNow,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns(channelReadModel);
        _youtubeCategoriesQuery
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(categoryReadModel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _dailyCategoryWatchCountersRepository
            .DidNotReceive()
            .GetByUserIdDateAndCategoryAsync(
                Arg.Any<UserId>(),
                Arg.Any<DateOnly>(),
                Arg.Any<YoutubeCategoryId>(),
                Arg.Any<CancellationToken>()
            );
    }
}
