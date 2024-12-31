using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class RemoveFoodDiaryCommandHandlerTests : IDisposable
{
    private readonly IFoodDiaryRepository foodDiaryRepository =
        Substitute.For<IFoodDiaryRepository>();
    private readonly IUserIdentifierProvider userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();

    private readonly DeleteFoodDiaryCommandHandler sut;

    public RemoveFoodDiaryCommandHandlerTests()
    {
        sut = new DeleteFoodDiaryCommandHandler(foodDiaryRepository, userIdentifierProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodDiaryRepository.ClearSubstitute();
        userIdentifierProvider.ClearSubstitute();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        // Arrange

        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            food: food,
            servingSize: servingSize
        );

        var command = new RemoveFoodDiaryCommand(foodDiary.Id);

        foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        foodDiaryRepository.Received(1).Remove(foodDiary);
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryEntryNotFound_ShouldReturnFailureResult()
    {
        // Arrange

        var command = new RemoveFoodDiaryCommand(NutritionDiaryId.NewId());

        foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((FoodDiary)null!);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotFound(command.Id));
        await foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        foodDiaryRepository.DidNotReceive().Remove(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerOfFoodDiaryEntry_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();
        var NutritionDiaryId = NutritionDiaryId.NewId();

        var foodDiary = FoodDiaryFaker.Generate();

        var command = new RemoveFoodDiaryCommand(NutritionDiaryId);

        foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotOwned(command.Id, userId));
        await foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());
        foodDiaryRepository.DidNotReceive().Remove(Arg.Any<FoodDiary>());
    }
}
