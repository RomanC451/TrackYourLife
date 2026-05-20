using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.AddChannelToCategory;

public sealed class AddChannelToCategoryCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ISubscriptionStatusProvider _subscriptionStatusProvider;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeCategoriesQuery _youtubeCategoriesQuery;
    private readonly IYoutubeApiService _youtubeApiService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AddChannelToCategoryCommandHandler _handler;

    public AddChannelToCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _subscriptionStatusProvider = Substitute.For<ISubscriptionStatusProvider>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeCategoriesQuery = Substitute.For<IYoutubeCategoriesQuery>();
        _youtubeApiService = Substitute.For<IYoutubeApiService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new AddChannelToCategoryCommandHandler(
            _userIdentifierProvider,
            _subscriptionStatusProvider,
            _youtubeChannelsRepository,
            _youtubeChannelsQuery,
            _youtubeCategoriesQuery,
            _youtubeApiService,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenChannelAlreadyExists_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new AddChannelToCategoryCommand(youtubeChannelId, categoryId);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.AlreadyExists(youtubeChannelId));
        await _youtubeChannelsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubeChannel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChannelInfoApiFails_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new AddChannelToCategoryCommand(youtubeChannelId, categoryId);
        var error = InfrastructureErrors.SupaBaseClient.ClientNotWorking;
        var utc = DateTime.UtcNow;
        var categoryRm = new YoutubeCategoryReadModel(
            categoryId,
            userId,
            "Edu",
            MaxVideosPerDay: 10,
            DisplayOrder: 0,
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeCategoriesQuery.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(categoryRm);
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubeCategoryReadModel> { categoryRm });
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);
        _youtubeApiService
            .GetChannelInfoAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeChannelSearchResult>(error));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        await _youtubeChannelsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<YoutubeChannel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllConditionsMet_CreatesChannel()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "channel-id";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new AddChannelToCategoryCommand(youtubeChannelId, categoryId);
        var utcNow = DateTime.UtcNow;
        var channelInfo = new YoutubeChannelSearchResult(
            youtubeChannelId,
            "Channel Name",
            "Description",
            "thumbnail-url",
            1000,
            false
        );

        var categoryRm = new YoutubeCategoryReadModel(
            categoryId,
            userId,
            "Edu",
            MaxVideosPerDay: 10,
            DisplayOrder: 0,
            CreatedOnUtc: utcNow,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeChannelsQuery
            .ExistsByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeCategoriesQuery.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(categoryRm);
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubeCategoryReadModel> { categoryRm });
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);
        _youtubeApiService
            .GetChannelInfoAsync(youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(Result.Success(channelInfo));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeChannelsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeChannel>(c =>
                    c.YoutubeChannelId == youtubeChannelId && c.YoutubeCategoryId == categoryId
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
