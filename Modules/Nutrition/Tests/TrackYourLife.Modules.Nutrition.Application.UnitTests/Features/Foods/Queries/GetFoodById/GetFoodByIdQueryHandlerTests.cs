using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Queries.GetFoodById;

public class GetFoodByIdQueryHandlerTests
{
    private readonly IFoodQuery _foodQuery;
    private readonly IFoodHistoryQuery _foodHistoryQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetFoodByIdQueryHandler _handler;

    public GetFoodByIdQueryHandlerTests()
    {
        _foodQuery = Substitute.For<IFoodQuery>();
        _foodHistoryQuery = Substitute.For<IFoodHistoryQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetFoodByIdQueryHandler(
            _foodQuery,
            _foodHistoryQuery,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenFoodExists_ShouldReturnFoodDto()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var userId = UserId.NewId();
        var query = new GetFoodByIdQuery(foodId);
        var expectedFood = FoodFaker.GenerateReadModel();
        var expectedDto = expectedFood.ToDto();

        _userIdentifierProvider.UserId.Returns(userId);
        _foodQuery.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(expectedFood);
        _foodHistoryQuery
            .GetByUserAndFoodAsync(userId, foodId, Arg.Any<CancellationToken>())
            .Returns((FoodHistoryReadModel?)null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_WhenFoodExistsAndInHistory_ShouldReturnFoodDtoWithHistoryData()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var userId = UserId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var query = new GetFoodByIdQuery(foodId);
        var expectedFood = FoodFaker.GenerateReadModel();
        var history = new FoodHistoryReadModel(
            FoodHistoryId.NewId(),
            userId,
            foodId,
            DateTime.UtcNow,
            servingSizeId,
            2.5f
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _foodQuery.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(expectedFood);
        _foodHistoryQuery
            .GetByUserAndFoodAsync(userId, foodId, Arg.Any<CancellationToken>())
            .Returns(history);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.LastServingSizeUsedId.Should().Be(servingSizeId);
        result.Value.LastQuantityUsed.Should().Be(2.5f);
    }

    [Fact]
    public async Task Handle_WhenFoodDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var userId = UserId.NewId();
        var query = new GetFoodByIdQuery(foodId);

        _userIdentifierProvider.UserId.Returns(userId);
        _foodQuery.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns((FoodReadModel?)null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundById(foodId));
    }
}
