using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.UpdateFoodDiary;

public class UpdateFoodDiaryCommandHandlerTests
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IServingSizeRepository _servingSizeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly UpdateFoodDiaryCommandHandler _handler;

    private readonly UserId _userId;
    private readonly NutritionDiaryId _foodDiaryId;
    private readonly ServingSizeId _servingSizeId;
    private readonly MealTypes _mealType;
    private readonly float _quantity;

    public UpdateFoodDiaryCommandHandlerTests()
    {
        _foodDiaryRepository = Substitute.For<IFoodDiaryRepository>();
        _foodRepository = Substitute.For<IFoodRepository>();
        _servingSizeRepository = Substitute.For<IServingSizeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new UpdateFoodDiaryCommandHandler(
            _foodDiaryRepository,
            _foodRepository,
            _servingSizeRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _foodDiaryId = NutritionDiaryId.NewId();
        _servingSizeId = ServingSizeId.NewId();
        _mealType = MealTypes.Breakfast;
        _quantity = 1.5f;

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryNotFound_ShouldReturnFailure()
    {
        // Arrange
        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns((FoodDiary?)null);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodDiaryErrors.NotFound(_foodDiaryId));
        _foodDiaryRepository.DidNotReceive().Update(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryNotOwnedByUser_ShouldReturnFailure()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: UserId.NewId(), // Different user
            quantity: 1.5f,
            date: DateOnly.FromDateTime(DateTime.Today),
            mealType: MealTypes.Breakfast
        );

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodDiaryErrors.NotOwned(_foodDiaryId));
        _foodDiaryRepository.DidNotReceive().Update(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ShouldReturnFailure()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: _userId,
            quantity: 1.5f,
            date: DateOnly.FromDateTime(DateTime.Today),
            mealType: MealTypes.Breakfast
        );

        var servingSize = ServingSizeFaker.Generate(_servingSizeId);

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _foodRepository
            .GetByIdAsync(foodDiary.FoodId, Arg.Any<CancellationToken>())
            .Returns((Food?)null);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundById(foodDiary.FoodId));
        _foodDiaryRepository.DidNotReceive().Update(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotFound_ShouldReturnFailure()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: _userId,
            quantity: 1.5f,
            date: DateOnly.FromDateTime(DateTime.Today),
            mealType: MealTypes.Breakfast
        );

        var food = FoodFaker.Generate();

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _foodRepository.GetByIdAsync(foodDiary.FoodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns((ServingSize?)null);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ServingSizeErrors.NotFound(_servingSizeId));
        _foodDiaryRepository.DidNotReceive().Update(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotApplicableToFood_ShouldReturnFailure()
    {
        // Arrange
        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: _userId,
            quantity: 1.5f,
            date: DateOnly.FromDateTime(DateTime.Today),
            mealType: MealTypes.Breakfast
        );

        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.Generate();

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _foodRepository.GetByIdAsync(foodDiary.FoodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.ServingSizeNotFound(food.Id, _servingSizeId));
        _foodDiaryRepository.DidNotReceive().Update(Arg.Any<FoodDiary>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldUpdateFoodDiary()
    {
        // Arrange
        FoodDiary updatedFoodDiary = null!;

        var foodDiary = FoodDiaryFaker.Generate(
            id: _foodDiaryId,
            userId: _userId,
            quantity: 1.5f,
            date: DateOnly.FromDateTime(DateTime.Today),
            mealType: MealTypes.Breakfast
        );

        var food = FoodFaker.Generate();
        var servingSize = ServingSizeFaker.Generate(_servingSizeId);

        food.FoodServingSizes.Add(FoodServingSizeFaker.Generate(0, food.Id, servingSize));

        _foodDiaryRepository
            .GetByIdAsync(_foodDiaryId, Arg.Any<CancellationToken>())
            .Returns(foodDiary);

        _foodDiaryRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<FoodDiary>()))
            .Do(x => updatedFoodDiary = x.Arg<FoodDiary>());

        _foodRepository.GetByIdAsync(foodDiary.FoodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new UpdateFoodDiaryCommand(
            _foodDiaryId,
            _quantity,
            _servingSizeId,
            _mealType
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodDiaryRepository.Received(1).Update(updatedFoodDiary);

        updatedFoodDiary.Should().NotBeNull();
        updatedFoodDiary.Id.Should().Be(_foodDiaryId);
        updatedFoodDiary.Quantity.Should().Be(_quantity);
        updatedFoodDiary.ServingSizeId.Should().Be(_servingSizeId);
        updatedFoodDiary.MealType.Should().Be(_mealType);
    }
}
