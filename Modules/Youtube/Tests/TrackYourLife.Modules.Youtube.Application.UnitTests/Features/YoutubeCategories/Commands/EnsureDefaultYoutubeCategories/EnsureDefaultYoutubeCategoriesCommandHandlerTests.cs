using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;

public sealed class EnsureDefaultYoutubeCategoriesCommandHandlerTests
{
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly EnsureDefaultYoutubeCategoriesCommandHandler _handler;

    public EnsureDefaultYoutubeCategoriesCommandHandlerTests()
    {
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new EnsureDefaultYoutubeCategoriesCommandHandler(
            _youtubeCategoriesRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenCategoriesAlreadyExist_DoesNothing()
    {
        var userId = UserId.NewId();
        var command = new EnsureDefaultYoutubeCategoriesCommand(userId);

        _youtubeCategoriesRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(2);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeCategoriesRepository.DidNotReceive().AddAsync(Arg.Any<YoutubeCategory>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoCategoriesExist_SeedsDefaults()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var command = new EnsureDefaultYoutubeCategoriesCommand(userId);

        _youtubeCategoriesRepository.CountByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(0);
        _dateTimeProvider.UtcNow.Returns(utcNow);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeCategoriesRepository
            .Received(2)
            .AddAsync(Arg.Any<YoutubeCategory>(), Arg.Any<CancellationToken>());
        await _youtubeCategoriesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeCategory>(c =>
                    c.Name == YoutubeCategoryDefaults.EntertainmentName
                    && c.MaxVideosPerDay == YoutubeCategoryDefaults.EntertainmentMaxVideosPerDay
                ),
                Arg.Any<CancellationToken>()
            );
        await _youtubeCategoriesRepository
            .Received(1)
            .AddAsync(
                Arg.Is<YoutubeCategory>(c =>
                    c.Name == YoutubeCategoryDefaults.EducationalName
                    && c.MaxVideosPerDay == YoutubeCategoryDefaults.EducationalMaxVideosPerDay
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
