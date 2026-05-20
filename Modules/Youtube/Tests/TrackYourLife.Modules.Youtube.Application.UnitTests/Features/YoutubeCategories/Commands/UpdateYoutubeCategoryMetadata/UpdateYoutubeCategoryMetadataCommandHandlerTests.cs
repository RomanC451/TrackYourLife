using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryMetadata;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryMetadata;

public sealed class UpdateYoutubeCategoryMetadataCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeCategoriesRepository _youtubeCategoriesRepository;
    private readonly IYoutubeChannelsRepository _youtubeChannelsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdateYoutubeCategoryMetadataCommandHandler _handler;

    public UpdateYoutubeCategoryMetadataCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeCategoriesRepository = Substitute.For<IYoutubeCategoriesRepository>();
        _youtubeChannelsRepository = Substitute.For<IYoutubeChannelsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new UpdateYoutubeCategoryMetadataCommandHandler(
            _userIdentifierProvider,
            _youtubeCategoriesRepository,
            _youtubeChannelsRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var command = new UpdateYoutubeCategoryMetadataCommand(categoryId, "New Name", 1);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository
            .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((YoutubeCategory?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenDuplicateName_ReturnsFailure()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var category = YoutubeCategory.Create(categoryId, userId, "Old", 3, 0, DateTime.UtcNow).Value;
        var command = new UpdateYoutubeCategoryMetadataCommand(categoryId, "Taken", 1);

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeCategoriesRepository
            .ExistsByUserIdAndNameIgnoreCaseAsync(userId, "Taken", categoryId, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeCategoriesErrors.DuplicateName);
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesCategoryAndSyncsChannelNames()
    {
        var userId = UserId.NewId();
        var categoryId = YoutubeCategoryId.NewId();
        var utcNow = DateTime.UtcNow;
        var category = YoutubeCategory.Create(categoryId, userId, "Old", 3, 0, utcNow).Value;
        var channel = YoutubeChannel
            .Create(
                YoutubeChannelId.NewId(),
                userId,
                "UCtest",
                "Channel",
                null,
                categoryId,
                "Old",
                utcNow
            )
            .Value;
        var command = new UpdateYoutubeCategoryMetadataCommand(categoryId, "Renamed", 2);

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow.AddMinutes(1));
        _youtubeCategoriesRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _youtubeCategoriesRepository
            .ExistsByUserIdAndNameIgnoreCaseAsync(userId, "Renamed", categoryId, Arg.Any<CancellationToken>())
            .Returns(false);
        _youtubeChannelsRepository
            .ListByUserIdAndCategoryIdAsync(userId, categoryId, Arg.Any<CancellationToken>())
            .Returns(new List<YoutubeChannel> { channel });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        category.Name.Should().Be("Renamed");
        category.DisplayOrder.Should().Be(2);
        channel.CategoryName.Should().Be("Renamed");
        _youtubeCategoriesRepository.Received(1).Update(category);
        _youtubeChannelsRepository.Received(1).Update(channel);
    }
}
