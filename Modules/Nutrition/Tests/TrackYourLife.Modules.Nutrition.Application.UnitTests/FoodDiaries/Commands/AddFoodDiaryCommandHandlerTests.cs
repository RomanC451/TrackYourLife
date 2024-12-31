using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Commands;

public class AddFoodDiaryCommandHandlerTests : IDisposable
{
    private readonly IFoodDiaryRepository foodDiaryRepository =
        Substitute.For<IFoodDiaryRepository>();
    private readonly IServingSizeRepository servingSizeRepository =
        Substitute.For<IServingSizeRepository>();
    private readonly IUserIdentifierProvider userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();
    private readonly IFoodRepository foodRepository = Substitute.For<IFoodRepository>();

    private readonly AddFoodDiaryCommandHandler sut;

    public AddFoodDiaryCommandHandlerTests()
    {
        sut = new AddFoodDiaryCommandHandler(
            foodDiaryRepository,
            foodRepository,
            servingSizeRepository,
            userIdentifierProvider
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodDiaryRepository.ClearSubstitute();
        servingSizeRepository.ClearSubstitute();
        userIdentifierProvider.ClearSubstitute();
        foodRepository.ClearSubstitute();
    }

    [Fact]
    public async Task Handler_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange

        var servingSize = ServingSizeFaker.Generate();

        var foodId = FoodId.NewId();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: servingSize)
            ]
        );

        var command = new AddFoodDiaryCommand(
            foodId,
            MealTypes.Breakfast,
            servingSize.Id,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);

        servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<NutritionDiaryId>();

        await foodDiaryRepository
            .Received(1)
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());

        await foodRepository.Received(1).GetByIdAsync(foodId, Arg.Any<CancellationToken>());

        await servingSizeRepository
            .Received(1)
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handler_WithInvalidFoodId_ShouldFail()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();

        var command = new AddFoodDiaryCommand(
            FoodId.Empty,
            MealTypes.Breakfast,
            servingSize.Id,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        foodRepository
            .GetByIdAsync(command.FoodId, Arg.Any<CancellationToken>())
            .Returns((Food)null!);

        servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");

        await foodRepository.Received(1).GetByIdAsync(command.FoodId, Arg.Any<CancellationToken>());
        await servingSizeRepository
            .DidNotReceive()
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
        await foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handler_WithInvalidServingSizeId_ShouldFail()
    {
        // Arrange

        var servingSize = ServingSizeFaker.Generate();

        var foodId = FoodId.NewId();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, foodId: foodId, servingSize: servingSize)
            ]
        );

        var command = new AddFoodDiaryCommand(
            foodId,
            MealTypes.Breakfast,
            ServingSizeId.Empty,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);

        servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns((ServingSize)null!);

        userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");

        await foodRepository.Received(1).GetByIdAsync(foodId, Arg.Any<CancellationToken>());
        await servingSizeRepository
            .Received(1)
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
        await foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handler_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var foodId = FoodId.NewId();

        var servingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
                FoodServingSizeFaker.Generate(1)
            ]
        );

        var command = new AddFoodDiaryCommand(
            foodId,
            MealTypes.Breakfast,
            servingSize.Id,
            0,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);

        servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");

        await foodRepository.Received(1).GetByIdAsync(foodId, Arg.Any<CancellationToken>());
        await servingSizeRepository
            .Received(1)
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
        await foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithNotExistingServingSizeInFood_ShouldReturnFailureResult()
    {
        // Arrange

        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, servingSize: ServingSizeFaker.Generate())
            ]
        );

        var command = new AddFoodDiaryCommand(
            foodId,
            MealTypes.Breakfast,
            servingSize.Id,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        foodRepository.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(food);

        servingSizeRepository
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        userIdentifierProvider.UserId.Returns(UserId.NewId());

        // Act
        var result = await sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("ServingSizeNotFound");

        await foodRepository.Received(1).GetByIdAsync(foodId, Arg.Any<CancellationToken>());
        await servingSizeRepository
            .Received(1)
            .GetByIdAsync(command.ServingSizeId, Arg.Any<CancellationToken>());
        await foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }
}
