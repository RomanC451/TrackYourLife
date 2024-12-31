using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class UpdateFoodDiaryCommandHandlerTests : IDisposable
{
    private readonly IFoodDiaryRepository _foodDiaryRepository =
        Substitute.For<IFoodDiaryRepository>();
    private readonly IServingSizeRepository _servingSizeRepository =
        Substitute.For<IServingSizeRepository>();
    private readonly IUserIdentifierProvider _userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();

    private readonly UpdateFoodDiaryCommandHandler sut;

    public UpdateFoodDiaryCommandHandlerTests()
    {
        sut = new UpdateFoodDiaryCommandHandler(
            _foodDiaryRepository,
            _servingSizeRepository,
            _userIdentifierProvider
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _foodDiaryRepository.ClearSubstitute();
        _servingSizeRepository.ClearSubstitute();
        _userIdentifierProvider.ClearSubstitute();
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var oldServingSize = ServingSizeFaker.Generate();
        var newServingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: oldServingSize),
                FoodServingSizeFaker.Generate(1, foodId: foodId, servingSize: newServingSize)
            ]
        );

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            food: food,
            servingSize: oldServingSize
        );

        var command = new UpdateFoodDiaryCommand(
            foodDiary.Id,
            1,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(newServingSize);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        foodDiary.Quantity.Should().Be(command.Quantity);
        foodDiary.ServingSize.Should().Be(newServingSize);
        foodDiary.MealType.Should().Be(command.MealType);

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .Received()
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNotExistingFoodDiary_ShouldReturnFailureResult()
    {
        // Arrange

        var command = new UpdateFoodDiaryCommand(
            NutritionDiaryId.NewId(),
            1,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((FoodDiary)null!);

        _userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotFound(command.Id));

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<ServingSizeId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNotCorrectUser_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();

        var foodDiary = FoodDiaryFaker.Generate();

        var command = new UpdateFoodDiaryCommand(
            foodDiary.Id,
            foodDiary.Quantity,
            ServingSizeId.NewId(),
            foodDiary.MealType
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeDiaryErrors.NotOwned(command.Id, userId));

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .DidNotReceive()
            .GetByIdAsync(Arg.Any<ServingSizeId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNotExistingServingSize_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();

        var foodDiary = FoodDiaryFaker.Generate(userId: userId);

        var command = new UpdateFoodDiaryCommand(
            foodDiary.Id,
            1,
            ServingSizeId.NewId(),
            MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns((ServingSize)null!);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ServingSizeErrors.NotFound(command.ServingSizeId));

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .Received(1)
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNotExistingServingSizeInFood_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        var oldServingSize = ServingSizeFaker.Generate();

        var newServingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: oldServingSize),
            ]
        );

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            food: food,
            servingSize: oldServingSize
        );

        var command = new UpdateFoodDiaryCommand(
            foodDiary.Id,
            1,
            newServingSize.Id,
            MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(newServingSize);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.ServingSizeNotFound(foodId, command.ServingSizeId));

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .Received()
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidQuantity_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        var servingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: servingSize)
            ]
        );

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            food: food,
            servingSize: servingSize
        );

        var command = new UpdateFoodDiaryCommand(
            foodDiary.Id,
            -1,
            servingSize.Id,
            MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .Received()
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithInvalidMealType_ShouldReturnFailureResult()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        var servingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: servingSize),
            ]
        );

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            food: food,
            servingSize: servingSize
        );

        var command = new UpdateFoodDiaryCommand(foodDiary.Id, 1, servingSize.Id, (MealTypes)100);

        _foodDiaryRepository
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _userIdentifierProvider.UserId.Returns(userId);

        _servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Invalid");

        await _foodDiaryRepository
            .Received(1)
            .GetByIdAsync(command.Id, Arg.Any<CancellationToken>());

        await _servingSizeRepository
            .Received()
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }
}
