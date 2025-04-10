using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients.Events;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodsHistory.Events;

public class IngredientCreatedDomainEventHandlerTests
{
    private readonly IFoodHistoryService _foodHistoryService;
    private readonly INutritionUnitOfWork _unitOfWork;
    private readonly IngredientCreatedDomainEventHandler _handler;

    public IngredientCreatedDomainEventHandlerTests()
    {
        _foodHistoryService = Substitute.For<IFoodHistoryService>();
        _unitOfWork = Substitute.For<INutritionUnitOfWork>();
        _handler = new IngredientCreatedDomainEventHandler(_foodHistoryService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddFoodToHistoryAndSaveChanges()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var notification = new IngredientCreatedDomainEvent(userId, foodId);

        // Act
        await _handler.Handle(notification, default);

        // Assert
        await _foodHistoryService
            .Received(1)
            .AddNewFoodAsync(userId, foodId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
