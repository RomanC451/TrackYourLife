using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;

public sealed class DeleteYoutubeCategoryCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly IYoutubeCategoriesQuery _youtubeCategoriesQuery;
    private readonly ISubscriptionStatusProvider _subscriptionStatusProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DeleteYoutubeCategoryCommandHandler _handler;

    public DeleteYoutubeCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _youtubeCategoriesQuery = Substitute.For<IYoutubeCategoriesQuery>();
        _subscriptionStatusProvider = Substitute.For<ISubscriptionStatusProvider>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new DeleteYoutubeCategoryCommandHandler(
            _userIdentifierProvider,
            _youtubeCategoriesRepository,
            _youtubeChannelsRepository,
            _youtubeChannelsQuery,
            _youtubeCategoriesQuery,
            _subscriptionStatusProvider,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var command = new DeleteYoutubeCategoryCommand(
            categoryId,
            ConfirmUnsubscribeChannels: false,
            MoveChannelsToCategoryId: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((YoutubeCategory?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasChannelsWithoutConfirmation_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Test", 3, 0, DateTime.UtcNow).Value;
        var command = new DeleteYoutubeCategoryCommand(
            categoryId,
            ConfirmUnsubscribeChannels: false,
            MoveChannelsToCategoryId: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(2);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.DeleteRequiresConfirmation);
        _youtubeCategoriesRepository.DidNotReceive().Remove(Arg.Any<YoutubeCategory>());
    }

    [Fact]
    public async Task Handle_WhenCategoryHasChannelsWithConfirmation_RemovesChannelsAndCategory()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Test", 3, 0, DateTime.UtcNow).Value;
        var command = new DeleteYoutubeCategoryCommand(
            categoryId,
            ConfirmUnsubscribeChannels: true,
            MoveChannelsToCategoryId: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeChannelsRepository
            .Received(1)
            .RemoveAllByUserIdAndCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>());
        _youtubeCategoriesRepository.Received(1).Remove(category);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasChannelsAndMoveTarget_MovesChannelsAndRemovesCategory()
    {
        var userId = UserId.NewId();
        var sourceCategoryId = YoutubeCategoryId.NewId();
        var targetCategoryId = YoutubeCategoryId.NewId();
        var sourceCategory = YoutubeCategory
            .Create(sourceCategoryId, userId, "Source", 0, 0, DateTime.UtcNow)
            .Value;
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                "UC1",
                "Channel",
                null,
                sourceCategoryId,
                "Source",
                DateTime.UtcNow
            )
            .Value;
        var utcNow = new DateTime(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
        var command = new DeleteYoutubeCategoryCommand(
            sourceCategoryId,
            ConfirmUnsubscribeChannels: false,
            MoveChannelsToCategoryId: targetCategoryId
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository
            .GetByIdAsync(sourceCategoryId, Arg.Any<CancellationToken>())
            .Returns(sourceCategory);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(
                userId,
                sourceCategoryId,
                Arg.Any<CancellationToken>()
            )
            .Returns(1);
        _youtubeCategoriesQuery
            .GetByIdAsync(targetCategoryId, Arg.Any<CancellationToken>())
            .Returns(
                new YoutubeCategoryReadModel(
                    targetCategoryId,
                    userId,
                    "Target",
                    1,
                    5,
                    DateTime.UtcNow,
                    null
                )
            );
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(
                [
                    new YoutubeCategoryReadModel(
                        sourceCategoryId,
                        userId,
                        "Source",
                        0,
                        5,
                        DateTime.UtcNow,
                        null
                    ),
                    new YoutubeCategoryReadModel(
                        targetCategoryId,
                        userId,
                        "Target",
                        1,
                        5,
                        DateTime.UtcNow,
                        null
                    ),
                ]
            );
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);
        _youtubeChannelsRepository
            .ListByUserIdAndCategoryIdAsync(userId, sourceCategoryId, Arg.Any<CancellationToken>())
            .Returns([channel]);
        _dateTimeProvider.UtcNow.Returns(utcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        channel.YoutubeCategoryId.Should().Be(targetCategoryId);
        channel.CategoryName.Should().Be("Target");
        _youtubeChannelsRepository.Received(1).Update(channel);
        await _youtubeChannelsRepository
            .DidNotReceive()
            .RemoveAllByUserIdAndCategoryIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<YoutubeCategoryId>(),
                Arg.Any<CancellationToken>()
            );
        _youtubeCategoriesRepository.Received(1).Remove(sourceCategory);
    }

    [Fact]
    public async Task Handle_WhenUnsubscribeAndMoveBothSpecified_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var targetCategoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Test", 3, 0, DateTime.UtcNow).Value;
        var command = new DeleteYoutubeCategoryCommand(
            categoryId,
            ConfirmUnsubscribeChannels: true,
            MoveChannelsToCategoryId: targetCategoryId
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.InvalidDeleteChannelDisposition);
        _youtubeCategoriesRepository.DidNotReceive().Remove(Arg.Any<YoutubeCategory>());
    }

    [Fact]
    public async Task Handle_WhenCategoryIsEmpty_RemovesCategory()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Test", 3, 0, DateTime.UtcNow).Value;
        var command = new DeleteYoutubeCategoryCommand(
            categoryId,
            ConfirmUnsubscribeChannels: false,
            MoveChannelsToCategoryId: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeChannelsRepository
            .DidNotReceive()
            .RemoveAllByUserIdAndCategoryIdAsync(
                Arg.Any<UserId>(),
                Arg.Any<YoutubeCategoryId>(),
                Arg.Any<CancellationToken>()
            );
        _youtubeCategoriesRepository.Received(1).Remove(category);
    }
}
