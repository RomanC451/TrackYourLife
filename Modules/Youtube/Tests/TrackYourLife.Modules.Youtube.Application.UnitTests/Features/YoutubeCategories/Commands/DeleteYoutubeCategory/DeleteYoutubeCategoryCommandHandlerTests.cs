using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;
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
    private readonly DeleteYoutubeCategoryCommandHandler _handler;

    public DeleteYoutubeCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();

        _handler = new DeleteYoutubeCategoryCommandHandler(
            _userIdentifierProvider,
            _youtubeCategoriesRepository,
            _youtubeChannelsRepository,
            _youtubeChannelsQuery
        );
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var command = new DeleteYoutubeCategoryCommand(categoryId, ConfirmUnsubscribeChannels: false);

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
        var command = new DeleteYoutubeCategoryCommand(categoryId, ConfirmUnsubscribeChannels: false);

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
        var command = new DeleteYoutubeCategoryCommand(categoryId, ConfirmUnsubscribeChannels: true);

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
    public async Task Handle_WhenCategoryIsEmpty_RemovesCategory()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Test", 3, 0, DateTime.UtcNow).Value;
        var command = new DeleteYoutubeCategoryCommand(categoryId, ConfirmUnsubscribeChannels: false);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeChannelsQuery
            .CountByUserIdAndYoutubeCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeChannelsRepository
            .DidNotReceive()
            .RemoveAllByUserIdAndCategoryIdAsync(Arg.Any<UserId>(), Arg.Any<YoutubeCategoryId>(), Arg.Any<CancellationToken>());
        _youtubeCategoriesRepository.Received(1).Remove(category);
    }
}
