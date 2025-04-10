using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Queries.GetFoodById;

public class GetFoodByIdQueryHandlerTests
{
    private readonly IFoodQuery _foodQuery;
    private readonly GetFoodByIdQueryHandler _handler;

    public GetFoodByIdQueryHandlerTests()
    {
        _foodQuery = Substitute.For<IFoodQuery>();
        _handler = new GetFoodByIdQueryHandler(_foodQuery);
    }

    [Fact]
    public async Task Handle_WhenFoodExists_ShouldReturnFood()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var query = new GetFoodByIdQuery(foodId);
        var expectedFood = FoodFaker.GenerateReadModel();

        _foodQuery.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns(expectedFood);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedFood);
    }

    [Fact]
    public async Task Handle_WhenFoodDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var query = new GetFoodByIdQuery(foodId);

        _foodQuery.GetByIdAsync(foodId, Arg.Any<CancellationToken>()).Returns((FoodReadModel?)null);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundById(foodId));
    }
}
