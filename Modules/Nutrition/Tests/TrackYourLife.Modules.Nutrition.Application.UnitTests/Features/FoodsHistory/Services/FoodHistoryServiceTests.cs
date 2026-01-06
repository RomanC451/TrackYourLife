using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.FoodsHistory.Services;

public class FoodHistoryServiceTests
{
    private readonly IFoodHistoryRepository _foodHistoryRepository;
    private readonly IFoodHistoryQuery _foodHistoryQuery;
    private readonly IFoodQuery _foodQuery;
    private readonly FoodHistoryService _sut;

    public FoodHistoryServiceTests()
    {
        _foodHistoryRepository = Substitute.For<IFoodHistoryRepository>();
        _foodHistoryQuery = Substitute.For<IFoodHistoryQuery>();
        _foodQuery = Substitute.For<IFoodQuery>();
        _sut = new FoodHistoryService(_foodHistoryRepository, _foodHistoryQuery, _foodQuery);
    }

    [Fact]
    public async Task AddNewFoodAsync_WhenHistoryExists_ShouldUpdateLastUsedAt()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 2.5f;
        var existingHistory = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, foodId, servingSizeId, quantity)
            .Value;
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns(existingHistory);

        // Act
        var result = await _sut.AddNewFoodAsync(
            userId,
            foodId,
            servingSizeId,
            quantity,
            cancellationToken
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodHistoryRepository
            .Received(1)
            .Update(Arg.Is<FoodHistory>(h => h.UserId == userId && h.FoodId == foodId));
    }

    [Fact]
    public async Task AddNewFoodAsync_WhenHistoryDoesNotExistAndUnderLimit_ShouldAddNewHistory()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns((FoodHistory?)null);
        _foodHistoryRepository.GetUserHistoryCountAsync(userId, cancellationToken).Returns(50);

        // Act
        var result = await _sut.AddNewFoodAsync(
            userId,
            foodId,
            servingSizeId,
            quantity,
            cancellationToken
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _foodHistoryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<FoodHistory>(h =>
                    h.UserId == userId
                    && h.FoodId == foodId
                    && h.LastServingSizeUsedId == servingSizeId
                    && Math.Abs(h.LastQuantityUsed - quantity) < 0.00001
                ),
                cancellationToken
            );
    }

    [Fact]
    public async Task AddNewFoodAsync_WhenHistoryDoesNotExistAndAtLimit_ShouldRemoveOldestAndAddNew()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 1.0f;
        var oldestHistory = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, FoodId.NewId(), servingSizeId, quantity)
            .Value;
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns((FoodHistory?)null);
        _foodHistoryRepository.GetUserHistoryCountAsync(userId, cancellationToken).Returns(100);
        _foodHistoryRepository
            .GetOldestByUserAsync(userId, cancellationToken)
            .Returns(oldestHistory);

        // Act
        var result = await _sut.AddNewFoodAsync(
            userId,
            foodId,
            servingSizeId,
            quantity,
            cancellationToken
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodHistoryRepository.Received(1).Remove(oldestHistory);
        await _foodHistoryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<FoodHistory>(h =>
                    h.UserId == userId
                    && h.FoodId == foodId
                    && h.LastServingSizeUsedId == servingSizeId
                    && Math.Abs(h.LastQuantityUsed - quantity) < 0.00001
                ),
                cancellationToken
            );
    }

    [Fact]
    public async Task GetFoodHistoryAsync_ShouldReturnOrderedFoodDtosByTimeDifference()
    {
        // Arrange
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;
        var currentTime = DateTime.UtcNow;
        var servingSizeId = ServingSizeId.NewId();
        var food1 = new FoodReadModel(FoodId.NewId(), "Food 1", "Type 1", "Brand 1", "US");
        var food2 = new FoodReadModel(FoodId.NewId(), "Food 2", "Type 2", "Brand 2", "US");
        var history1 = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, food1.Id, servingSizeId, 1.0f)
            .Value;
        var history2 = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, food2.Id, servingSizeId, 2.0f)
            .Value;

        var historyReadModel1 = new FoodHistoryReadModel(
            history1.Id,
            history1.UserId,
            history1.FoodId,
            currentTime.AddHours(-1),
            servingSizeId,
            1.0f
        );
        var historyReadModel2 = new FoodHistoryReadModel(
            history2.Id,
            history2.UserId,
            history2.FoodId,
            currentTime.AddHours(-2),
            servingSizeId,
            2.0f
        );

        _foodHistoryQuery
            .GetHistoryByUserAsync(userId, cancellationToken)
            .Returns(new[] { historyReadModel1, historyReadModel2 });
        _foodQuery
            .GetFoodsPartOfAsync(Arg.Any<IEnumerable<FoodId>>(), cancellationToken)
            .Returns(new List<FoodReadModel> { food1, food2 });

        // Act
        var result = await _sut.GetFoodHistoryAsync(userId, cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        var resultList = result.ToList();
        resultList[0].Id.Should().Be(food1.Id);
        resultList[0].LastServingSizeUsedId.Should().Be(servingSizeId);
        resultList[0].LastQuantityUsed.Should().Be(1.0f);
        resultList[1].Id.Should().Be(food2.Id);
        resultList[1].LastServingSizeUsedId.Should().Be(servingSizeId);
        resultList[1].LastQuantityUsed.Should().Be(2.0f);
    }

    [Fact]
    public async Task PrioritizeHistoryItemsAsync_ShouldOrderHistoryItemsFirstAndReturnDtos()
    {
        // Arrange
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;
        var currentTime = DateTime.UtcNow;
        var servingSizeId = ServingSizeId.NewId();
        var food1 = new FoodReadModel(FoodId.NewId(), "Food 1", "Type 1", "Brand 1", "US");
        var food2 = new FoodReadModel(FoodId.NewId(), "Food 2", "Type 2", "Brand 2", "US");
        var food3 = new FoodReadModel(FoodId.NewId(), "Food 3", "Type 3", "Brand 3", "US");
        var history1 = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, food1.Id, servingSizeId, 1.0f)
            .Value;
        var history2 = FoodHistory
            .Create(FoodHistoryId.NewId(), userId, food2.Id, servingSizeId, 2.0f)
            .Value;

        var historyReadModel1 = new FoodHistoryReadModel(
            history1.Id,
            history1.UserId,
            history1.FoodId,
            currentTime.AddHours(-1),
            servingSizeId,
            1.0f
        );
        var historyReadModel2 = new FoodHistoryReadModel(
            history2.Id,
            history2.UserId,
            history2.FoodId,
            currentTime.AddHours(-2),
            servingSizeId,
            2.0f
        );

        var searchResults = new[] { food3, food1, food2 };
        _foodHistoryQuery
            .GetHistoryByUserAsync(userId, cancellationToken)
            .Returns(new[] { historyReadModel1, historyReadModel2 });

        // Act
        var result = await _sut.PrioritizeHistoryItemsAsync(
            userId,
            searchResults,
            cancellationToken
        );

        // Assert
        result.Should().HaveCount(3);
        var resultList = result.ToList();
        resultList[0].Id.Should().Be(food1.Id);
        resultList[0].LastServingSizeUsedId.Should().Be(servingSizeId);
        resultList[0].LastQuantityUsed.Should().Be(1.0f);
        resultList[1].Id.Should().Be(food2.Id);
        resultList[1].LastServingSizeUsedId.Should().Be(servingSizeId);
        resultList[1].LastQuantityUsed.Should().Be(2.0f);
        resultList[2].Id.Should().Be(food3.Id);
        resultList[2].LastServingSizeUsedId.Should().BeNull();
        resultList[2].LastQuantityUsed.Should().BeNull();
    }
}
