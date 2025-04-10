using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.RecipeDiaries.Commands.DeleteRecipeDiary;

public class DeleteRecipeDiaryCommandHandlerTests
{
    private readonly IRecipeDiaryRepository _recipeDiaryRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteRecipeDiaryCommandHandler _handler;

    public DeleteRecipeDiaryCommandHandlerTests()
    {
        _recipeDiaryRepository = Substitute.For<IRecipeDiaryRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteRecipeDiaryCommandHandler(
            _recipeDiaryRepository,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryExists_ShouldRemoveRecipeDiaryAndReturnSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new DeleteRecipeDiaryCommand(nutritionDiaryId);

        var recipeDiary = RecipeDiaryFaker.Generate(id: nutritionDiaryId, userId: userId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _recipeDiaryRepository
            .Received(1)
            .Remove(Arg.Is<RecipeDiary>(rd => rd.Id == nutritionDiaryId));
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new DeleteRecipeDiaryCommand(nutritionDiaryId);

        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns((RecipeDiary?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotFound(nutritionDiaryId));
        _recipeDiaryRepository.DidNotReceive().Remove(Arg.Any<RecipeDiary>());
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryBelongsToDifferentUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var otherUserId = UserId.NewId();
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var command = new DeleteRecipeDiaryCommand(nutritionDiaryId);

        var recipeDiary = RecipeDiaryFaker.Generate(id: nutritionDiaryId, userId: otherUserId);
        _userIdentifierProvider.UserId.Returns(userId);
        _recipeDiaryRepository
            .GetByIdAsync(nutritionDiaryId, Arg.Any<CancellationToken>())
            .Returns(recipeDiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotOwned(nutritionDiaryId));
        _recipeDiaryRepository.DidNotReceive().Remove(Arg.Any<RecipeDiary>());
    }
}
