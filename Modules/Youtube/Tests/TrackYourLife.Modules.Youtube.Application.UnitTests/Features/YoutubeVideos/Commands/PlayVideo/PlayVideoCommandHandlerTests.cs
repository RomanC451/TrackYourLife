using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
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
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IDailyEntertainmentCountersRepository _dailyEntertainmentCountersRepository;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly PlayVideoCommandHandler _handler;

    public PlayVideoCommandHandlerTests()
    {
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _watchedVideosRepository = Substitute.For<IWatchedVideosRepository>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _dailyEntertainmentCountersRepository =
            Substitute.For<IDailyEntertainmentCountersRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new PlayVideoCommandHandler(
            _youtubeApiService,
            _userIdentifierProvider,
            _watchedVideosRepository,
            _youtubeSettingsRepository,
            _dailyEntertainmentCountersRepository,
            _youtubeChannelsQuery,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenVideoAlreadyWatched_ReturnsVideoDetails()
    {
        // Arrange
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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(videoDetails);
        await _watchedVideosRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<WatchedVideo>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenVideoDetailsApiFails_ReturnsFailure()
    {
        // Arrange
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

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public async Task Handle_WhenEntertainmentLimitReached_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
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

        var settings = Domain
            .Features.YoutubeSettings.YoutubeSetting.Create(
                YoutubeSettingsId.NewId(),
                userId,
                maxEntertainmentVideosPerDay: 5,
                settingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
                daysBetweenChanges: 1,
                lastSettingsChangeUtc: utcNow,
                specificDayOfWeek: null,
                specificDayOfMonth: null,
                createdOnUtc: utcNow
            )
            .Value;

        var counter = DailyEntertainmentCounter
            .Create(DailyEntertainmentCounterId.NewId(), userId, today, 5)
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _dailyEntertainmentCountersRepository
            .GetByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns(counter);
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannelReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.EntertainmentLimitReached);
    }

    [Fact]
    public async Task Handle_WhenVideoNotWatchedAndLimitNotReached_CreatesWatchedVideo()
    {
        // Arrange
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
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

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Domain.Features.YoutubeSettings.YoutubeSetting?)null);
        _dailyEntertainmentCountersRepository
            .GetByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns((DailyEntertainmentCounter?)null);
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns((YoutubeChannelReadModel?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _watchedVideosRepository
            .Received(1)
            .AddAsync(
                Arg.Is<WatchedVideo>(w => w.VideoId == videoId),
                Arg.Any<CancellationToken>()
            );
        await _dailyEntertainmentCountersRepository
            .Received(1)
            .AddAsync(Arg.Any<DailyEntertainmentCounter>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChannelIsNotEntertainment_DoesNotIncrementCounter()
    {
        // Arrange
        var userId = UserId.NewId();
        var videoId = "test-video-id";
        var channelId = "channel-id";
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

        var counter = DailyEntertainmentCounter
            .Create(DailyEntertainmentCounterId.NewId(), userId, today, 0)
            .Value;

        var channelReadModel = new YoutubeChannelReadModel(
            YoutubeChannelId.NewId(),
            userId,
            channelId,
            "Channel Name",
            "thumbnail-url",
            VideoCategory.Educational,
            DateTime.UtcNow,
            null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _watchedVideosRepository
            .GetByUserIdAndVideoIdAsync(userId, videoId, Arg.Any<CancellationToken>())
            .Returns((WatchedVideo?)null);
        _youtubeApiService
            .GetVideoDetailsAsync(videoId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(videoDetails));
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Domain.Features.YoutubeSettings.YoutubeSetting?)null);
        _dailyEntertainmentCountersRepository
            .GetByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns(counter);
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeChannelIdAsync(userId, channelId, Arg.Any<CancellationToken>())
            .Returns(channelReadModel);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        counter.VideosWatchedCount.Should().Be(0);
    }
}
