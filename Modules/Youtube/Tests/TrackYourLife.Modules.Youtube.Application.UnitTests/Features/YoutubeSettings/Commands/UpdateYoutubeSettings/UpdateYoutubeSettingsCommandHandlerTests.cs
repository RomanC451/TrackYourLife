using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

public sealed class UpdateYoutubeSettingsCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdateYoutubeSettingsCommandHandler _handler;

    public UpdateYoutubeSettingsCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new UpdateYoutubeSettingsCommandHandler(
            _userIdentifierProvider,
            _youtubeSettingsRepository,
            _youtubeCategoriesRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenSettingsDoNotExist_CreatesNewSettings()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var cat1 = YoutubeCategory.Create(YoutubeCategoryId.NewId(), userId, "A", 3, 0, utcNow).Value;
        var categories = new List<YoutubeCategory> { cat1 };

        var command = new UpdateYoutubeSettingsCommand(
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeCategoriesRepository
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);
        _youtubeSettingsRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((YoutubeSetting?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _youtubeSettingsRepository.Received(1).AddAsync(Arg.Any<YoutubeSetting>(), Arg.Any<CancellationToken>());
        _youtubeCategoriesRepository.DidNotReceive().Update(Arg.Any<YoutubeCategory>());
    }

    [Fact]
    public async Task Handle_WhenSettingsCannotBeChanged_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var utcNow = new DateTime(2024, 1, 2, 12, 0, 0, DateTimeKind.Utc);
        var cat1 = YoutubeCategory.Create(YoutubeCategoryId.NewId(), userId, "A", 3, 0, utcNow).Value;
        var categories = new List<YoutubeCategory> { cat1 };

        var command = new UpdateYoutubeSettingsCommand(
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        var existingSettings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                userId,
                settingsChangeFrequency: SettingsChangeFrequency.SpecificDayOfWeek,
                daysBetweenChanges: null,
                lastSettingsChangeUtc: utcNow.AddDays(-1),
                specificDayOfWeek: DayOfWeek.Monday,
                specificDayOfMonth: null,
                createdOnUtc: utcNow.AddDays(-1)
            )
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeCategoriesRepository
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);
        _youtubeSettingsRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingSettings);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _youtubeSettingsRepository.DidNotReceive().Update(Arg.Any<YoutubeSetting>());
    }

    [Fact]
    public async Task Handle_WhenSettingsCanBeChanged_UpdatesSettings()
    {
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var cat1 = YoutubeCategory.Create(YoutubeCategoryId.NewId(), userId, "A", 3, 0, utcNow).Value;
        var categories = new List<YoutubeCategory> { cat1 };

        var command = new UpdateYoutubeSettingsCommand(
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        var existingSettings = YoutubeSetting
            .Create(
                YoutubeSettingsId.NewId(),
                userId,
                settingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
                daysBetweenChanges: 1,
                lastSettingsChangeUtc: utcNow.AddDays(-2),
                specificDayOfWeek: null,
                specificDayOfMonth: null,
                createdOnUtc: utcNow.AddDays(-2)
            )
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeCategoriesRepository
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);
        _youtubeSettingsRepository.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(existingSettings);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _youtubeSettingsRepository.Received(1).Update(existingSettings);
        _youtubeCategoriesRepository.DidNotReceive().Update(Arg.Any<YoutubeCategory>());
    }
}
