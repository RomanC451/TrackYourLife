using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodsHistory.Events;

public class FoodDiaryCreatedDomainEventHandlerTests
{
    private readonly IFoodHistoryService _foodHistoryService;
    private readonly INutritionUnitOfWork _unitOfWork;
    private readonly FoodDiaryCreatedDomainEventHandler _handler;

    public FoodDiaryCreatedDomainEventHandlerTests()
    {
        _foodHistoryService = Substitute.For<IFoodHistoryService>();
        _unitOfWork = Substitute.For<INutritionUnitOfWork>();
        _handler = new FoodDiaryCreatedDomainEventHandler(_foodHistoryService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddFoodToHistoryAndSaveChanges()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var notification = new FoodDiaryCreatedDomainEvent(
            userId,
            foodId,
            DateOnly.FromDateTime(DateTime.Now),
            ServingSizeId.NewId(),
            1
        );

        // Act
        await _handler.Handle(notification, default);

        // Assert
        await _foodHistoryService
            .Received(1)
            .AddNewFoodAsync(userId, foodId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
