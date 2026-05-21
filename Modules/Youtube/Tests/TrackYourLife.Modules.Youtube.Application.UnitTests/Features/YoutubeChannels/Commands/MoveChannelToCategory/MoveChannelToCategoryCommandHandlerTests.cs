using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.MoveChannelToCategory;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Commands.MoveChannelToCategory;

public sealed class MoveChannelToCategoryCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ISubscriptionStatusProvider _subscriptionStatusProvider;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IYoutubeCategoriesQuery _youtubeCategoriesQuery;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly MoveChannelToCategoryCommandHandler _handler;

    public MoveChannelToCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _subscriptionStatusProvider = Substitute.For<ISubscriptionStatusProvider>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _youtubeCategoriesQuery = Substitute.For<IYoutubeCategoriesQuery>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new MoveChannelToCategoryCommandHandler(
            _userIdentifierProvider,
            _subscriptionStatusProvider,
            _youtubeChannelsRepository,
            _youtubeCategoriesQuery,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenChannelNotFound_ReturnsFailure()
    {
        var youtubeChannelId = "UCmissing";
        var targetId = YoutubeCategoryId.NewId();
        var command = new MoveChannelToCategoryCommand(youtubeChannelId, targetId);

        _userIdentifierProvider.UserId.Returns(UserId.NewId());
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(
                _userIdentifierProvider.UserId,
                youtubeChannelId,
                Arg.Any<CancellationToken>()
            )
            .Returns((YoutubeChannel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeChannelsErrors.NotFound(youtubeChannelId));
        _youtubeChannelsRepository.DidNotReceive().Update(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenAlreadyInTargetCategory_ReturnsSuccessWithoutUpdate()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "UCsame";
        var categoryId = YoutubeCategoryId.NewId();
        var command = new MoveChannelToCategoryCommand(youtubeChannelId, categoryId);
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                youtubeChannelId,
                "Name",
                null,
                categoryId,
                "Cat",
                DateTime.UtcNow
            )
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _youtubeChannelsRepository.DidNotReceive().Update(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenTargetCategoryMissing_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "UCmove";
        var sourceCat = YoutubeCategoryId.NewId();
        var targetCat = YoutubeCategoryId.NewId();
        var command = new MoveChannelToCategoryCommand(youtubeChannelId, targetCat);
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                youtubeChannelId,
                "Name",
                null,
                sourceCat,
                "Source",
                DateTime.UtcNow
            )
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);
        _youtubeCategoriesQuery.GetByIdAsync(targetCat, Arg.Any<CancellationToken>()).Returns((YoutubeCategoryReadModel?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.NotFound);
        _youtubeChannelsRepository.DidNotReceive().Update(Arg.Any<YoutubeChannel>());
    }

    [Fact]
    public async Task Handle_WhenValid_MovesAndUpdates()
    {
        var userId = UserId.NewId();
        var youtubeChannelId = "UCok";
        var sourceCat = YoutubeCategoryId.NewId();
        var targetCat = YoutubeCategoryId.NewId();
        var command = new MoveChannelToCategoryCommand(youtubeChannelId, targetCat);
        var utc = DateTime.UtcNow;
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                youtubeChannelId,
                "Name",
                null,
                sourceCat,
                "Source",
                utc
            )
            .Value;

        var targetRm = new YoutubeCategoryReadModel(
            targetCat,
            userId,
            "Target",
            MaxVideosPerDay: 5,
            DisplayOrder: 1,
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utc.AddMinutes(1));
        _youtubeChannelsRepository
            .GetByUserIdAndYoutubeChannelIdAsync(userId, youtubeChannelId, Arg.Any<CancellationToken>())
            .Returns(channel);
        _youtubeCategoriesQuery.GetByIdAsync(targetCat, Arg.Any<CancellationToken>()).Returns(targetRm);
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubeCategoryReadModel> { targetRm });
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _youtubeChannelsRepository.Received(1).Update(channel);
        channel.YoutubeCategoryId.Should().Be(targetCat);
        channel.CategoryName.Should().Be("Target");
    }
}
