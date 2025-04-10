using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.DeleteFoodDiary;

public class DeleteFoodDiaryCommandHandlerTests
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly DeleteFoodDiaryCommandHandler _handler;

    private readonly UserId _userId;
    private readonly NutritionDiaryId _foodDiaryId;

    public DeleteFoodDiaryCommandHandlerTests()
    {
        _foodDiaryRepository = Substitute.For<IFoodDiaryRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new DeleteFoodDiaryCommandHandler(_foodDiaryRepository, _userIdentifierProvider);

        _userId = UserId.NewId();
        _foodDiaryId = NutritionDiaryId.NewId();

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryNotFound_ShouldReturnFailure()
    {
        // Arrange
        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns((FoodDiary?)null);

        var command = new DeleteFoodDiaryCommand(_foodDiaryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodDiaryErrors.NotFound(_foodDiaryId));
        _foodDiaryRepository.DidNotReceive().Remove(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryNotOwnedByUser_ShouldReturnFailure()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: UserId.NewId() // Different user
        );

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        var command = new DeleteFoodDiaryCommand(_foodDiaryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodDiaryErrors.NotOwned(_foodDiaryId));
        _foodDiaryRepository.DidNotReceive().Remove(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryExistsAndOwnedByUser_ShouldRemoveAndReturnSuccess()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(id: _foodDiaryId, userId: _userId);

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        var command = new DeleteFoodDiaryCommand(_foodDiaryId);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodDiaryRepository.Received(1).Remove(foodDiary);
    }
}
