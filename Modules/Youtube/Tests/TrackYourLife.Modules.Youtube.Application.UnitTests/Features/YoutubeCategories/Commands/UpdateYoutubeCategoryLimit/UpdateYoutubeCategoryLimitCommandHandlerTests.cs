using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;

public sealed class UpdateYoutubeCategoryLimitCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdateYoutubeCategoryLimitCommandHandler _handler;

    public UpdateYoutubeCategoryLimitCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new UpdateYoutubeCategoryLimitCommandHandler(
            _userIdentifierProvider,
            _youtubeCategoriesRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var command = new UpdateYoutubeCategoryLimitCommand(categoryId, 5);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((YoutubeCategory?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _youtubeCategoriesRepository.DidNotReceive().Update(Arg.Any<YoutubeCategory>());
    }

    [Fact]
    public async Task Handle_WhenCategoryBelongsToUser_UpdatesMaxVideosPerDay()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var category = YoutubeCategory.Create(YoutubeCategoryId.NewId(), userId, "Entertainment", 3, 0, utcNow).Value;
        var command = new UpdateYoutubeCategoryLimitCommand(category.Id, 8);

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeCategoriesRepository
            .GetByIdAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        category.MaxVideosPerDay.Should().Be(8);
        _youtubeCategoriesRepository.Received(1).Update(category);
    }
}
