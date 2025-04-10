using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodDiaries.Commands.AddFoodDiary;

public class AddFoodDiaryCommandHandlerTests
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;
    private readonly IFoodRepository _foodRepository;
    private readonly IServingSizeRepository _servingSizeRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly AddFoodDiaryCommandHandler _handler;

    private readonly UserId _userId;
    private readonly FoodId _foodId;
    private readonly ServingSizeId _servingSizeId;
    private readonly DateOnly _entryDate;
    private readonly MealTypes _mealType;
    private readonly float _quantity;

    public AddFoodDiaryCommandHandlerTests()
    {
        _foodDiaryRepository = Substitute.For<IFoodDiaryRepository>();
        _foodRepository = Substitute.For<IFoodRepository>();
        _servingSizeRepository = Substitute.For<IServingSizeRepository>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new AddFoodDiaryCommandHandler(
            _foodDiaryRepository,
            _foodRepository,
            _servingSizeRepository,
            _userIdentifierProvider
        );

        _userId = UserId.NewId();
        _foodId = FoodId.NewId();
        _servingSizeId = ServingSizeId.NewId();
        _entryDate = DateOnly.FromDateTime(DateTime.Today);
        _mealType = MealTypes.Breakfast;
        _quantity = 1.5f;

        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ShouldReturnFailure()
    {
        // Arrange
        _foodRepository.GetByIdAsync(_foodId, Arg.Any<CancellationToken>()).Returns((Food?)null);

        var command = new AddFoodDiaryCommand(
            _foodId,
            _mealType,
            _servingSizeId,
            _quantity,
            _entryDate
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundById(_foodId));
        await _foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotFound_ShouldReturnFailure()
    {
        // Arrange
        var food = FoodFaker.Generate(
            id: _foodId,
            foodServingSizes: [FoodServingSize.Create(_foodId, _servingSizeId, 0).Value]
        );

        _foodRepository.GetByIdAsync(_foodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns((ServingSize?)null);

        var command = new AddFoodDiaryCommand(
            _foodId,
            _mealType,
            _servingSizeId,
            _quantity,
            _entryDate
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ServingSizeErrors.NotFound(_servingSizeId));
        await _foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotApplicableToFood_ShouldReturnFailure()
    {
        // Arrange
        var food = FoodFaker.Generate(
            id: _foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(0, _foodId, ServingSizeFaker.Generate()),
            ]
        );

        var servingSize = ServingSizeFaker.Generate(_servingSizeId);

        _foodRepository.GetByIdAsync(_foodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new AddFoodDiaryCommand(
            _foodId,
            _mealType,
            _servingSizeId,
            _quantity,
            _entryDate
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.ServingSizeNotFound(_foodId, _servingSizeId));
        await _foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var food = FoodFaker.Generate(
            id: _foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(
                    0,
                    _foodId,
                    ServingSizeFaker.Generate(_servingSizeId)
                ),
            ]
        );

        var servingSize = ServingSizeFaker.Generate(_servingSizeId);

        _foodRepository.GetByIdAsync(_foodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new AddFoodDiaryCommand(
            _foodId,
            _mealType,
            _servingSizeId,
            -1, // Invalid quantity to force creation failure
            _entryDate
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        await _foodDiaryRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<FoodDiary>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenAllValidationsPass_ShouldCreateFoodDiary()
    {
        // Arrange
        var food = FoodFaker.Generate(
            id: _foodId,
            foodServingSizes:
            [
                FoodServingSizeFaker.Generate(
                    0,
                    _foodId,
                    ServingSizeFaker.Generate(_servingSizeId)
                ),
            ]
        );

        var servingSize = ServingSizeFaker.Generate(_servingSizeId);

        _foodRepository.GetByIdAsync(_foodId, Arg.Any<CancellationToken>()).Returns(food);

        _servingSizeRepository
            .GetByIdAsync(_servingSizeId, Arg.Any<CancellationToken>())
            .Returns(servingSize);

        var command = new AddFoodDiaryCommand(
            _foodId,
            _mealType,
            _servingSizeId,
            _quantity,
            _entryDate
        );

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _foodDiaryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<FoodDiary>(fd =>
                    fd.UserId == _userId
                    && fd.FoodId == _foodId
                    && fd.ServingSizeId == _servingSizeId
                    && Math.Abs(fd.Quantity - _quantity) < 0.0001f
                    && fd.Date == _entryDate
                    && fd.MealType == _mealType
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
