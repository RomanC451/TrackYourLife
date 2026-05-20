using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;
using TrackYourLife.Modules.Youtube.Application.Options;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeCategories.Commands.CreateYoutubeCategory;

public sealed class CreateYoutubeCategoryCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly ISubscriptionStatusProvider _subscriptionStatusProvider;
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly CreateYoutubeCategoryCommandHandler _handler;

    public CreateYoutubeCategoryCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _subscriptionStatusProvider = Substitute.For<ISubscriptionStatusProvider>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        var options = Microsoft.Extensions.Options.Options.Create(
            new YoutubeModuleOptions { MaxCategoriesForPro = 10 }
        );
        _handler = new CreateYoutubeCategoryCommandHandler(
            _userIdentifierProvider,
            _subscriptionStatusProvider,
            _youtubeCategoriesRepository,
            options,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenFreeUserAtCategoryLimit_ReturnsForbidden()
    {
        var userId = UserId.NewId();
        var command = new CreateYoutubeCategoryCommand("Custom", 3);

        _userIdentifierProvider.UserId.Returns(userId);
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(false);
        _youtubeCategoriesRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(2);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.ForbiddenForPlan);
        await _youtubeCategoriesRepository.DidNotReceive().AddAsync(Arg.Any<YoutubeCategory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDuplicateName_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var command = new CreateYoutubeCategoryCommand("Custom", 3);

        _userIdentifierProvider.UserId.Returns(userId);
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);
        _youtubeCategoriesRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(2);
        _youtubeCategoriesRepository
            .ExistsByUserIdAndNameIgnoreCaseAsync(userId, "Custom", null, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.DuplicateName);
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesCategoryWithNextDisplayOrder()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var existing = YoutubeCategory.Create(YoutubeCategoryId.NewId(), userId, "A", 3, 1, utcNow).Value;
        var command = new CreateYoutubeCategoryCommand("Custom", 4);

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _subscriptionStatusProvider.IsProAsync(Arg.Any<CancellationToken>()).Returns(true);
        _youtubeCategoriesRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(1);
        _youtubeCategoriesRepository
            .ExistsByUserIdAndNameIgnoreCaseAsync(userId, "Custom", null, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeCategoriesRepository
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubeCategory> { existing });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeCategoriesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeCategory>(c =>
                    c.Name == "Custom" && c.MaxVideosPerDay == 4 && c.DisplayOrder == 2
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
