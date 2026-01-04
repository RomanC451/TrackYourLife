using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.AddChannelToCategory;

public sealed class AddChannelToCategoryCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AddChannelToCategoryCommandHandler _handler;

    public AddChannelToCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new AddChannelToCategoryCommandHandler(
            _userIdentifierProvider,
            _youtubeChannelsRepository,
            _youtubeChannelsQuery,
            _youtubeApiService,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenChannelAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var command = new AddChannelToCategoryCommand(youtubeChannelId, VideoCategory.Educational);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.AlreadyExists(youtubeChannelId));
        await _youtubeChannelsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubeChannel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChannelInfoApiFails_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var command = new AddChannelToCategoryCommand(youtubeChannelId, VideoCategory.Educational);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeApiService
            .GetChannelInfoAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeChannelSearchResult>(error));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        await _youtubeChannelsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubeChannel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllConditionsMet_CreatesChannel()
    {
        // Arrange
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var command = new AddChannelToCategoryCommand(youtubeChannelId, VideoCategory.Educational);
        var utcNow = DateTime.UtcNow;
        var channelInfo = new YoutubeChannelSearchResult(
            youtubeChannelId,
            "Channel Name",
            "Description",
            "thumbnail-url",
            1000
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeApiService
            .GetChannelInfoAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(channelInfo));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _youtubeChannelsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeChannel>(c =>
                    c.YoutubeChannelId == youtubeChannelId &&
                    c.Category == VideoCategory.Educational
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
