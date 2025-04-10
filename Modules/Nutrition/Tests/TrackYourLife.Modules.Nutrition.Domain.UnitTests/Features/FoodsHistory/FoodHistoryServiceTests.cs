using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.FoodsHistory;

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
        var existingHistory = FoodHistory.Create(FoodHistoryId.NewId(), userId, foodId).Value;
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns(existingHistory);

        // Act
        var result = await _sut.AddNewFoodAsync(userId, foodId, cancellationToken);

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
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns((FoodHistory?)null);
        _foodHistoryRepository.GetUserHistoryCountAsync(userId, cancellationToken).Returns(50);

        // Act
        var result = await _sut.AddNewFoodAsync(userId, foodId, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _foodHistoryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<FoodHistory>(h => h.UserId == userId && h.FoodId == foodId),
                cancellationToken
            );
    }

    [Fact]
    public async Task AddNewFoodAsync_WhenHistoryDoesNotExistAndAtLimit_ShouldRemoveOldestAndAddNew()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var oldestHistory = FoodHistory.Create(FoodHistoryId.NewId(), userId, FoodId.NewId()).Value;
        var cancellationToken = CancellationToken.None;

        _foodHistoryRepository
            .GetByUserAndFoodAsync(userId, foodId, cancellationToken)
            .Returns((FoodHistory?)null);
        _foodHistoryRepository.GetUserHistoryCountAsync(userId, cancellationToken).Returns(100);
        _foodHistoryRepository
            .GetOldestByUserAsync(userId, cancellationToken)
            .Returns(oldestHistory);

        // Act
        var result = await _sut.AddNewFoodAsync(userId, foodId, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _foodHistoryRepository.Received(1).Remove(oldestHistory);
        await _foodHistoryRepository
            .Received(1)
            .AddAsync(
                Arg.Is<FoodHistory>(h => h.UserId == userId && h.FoodId == foodId),
                cancellationToken
            );
    }

    [Fact]
    public async Task GetFoodHistoryAsync_ShouldReturnOrderedFoodsByTimeDifference()
    {
        // Arrange
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;
        var currentTime = DateTime.UtcNow;
        var food1 = new FoodReadModel(
            FoodId.NewId(),
            "Food 1",
            "Description 1",
            "Category 1",
            "Brand 1"
        );
        var food2 = new FoodReadModel(
            FoodId.NewId(),
            "Food 2",
            "Description 2",
            "Category 2",
            "Brand 2"
        );
        var history1 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food1.Id).Value;
        var history2 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food2.Id).Value;

        var historyReadModel1 = new FoodHistoryReadModel(
            history1.Id,
            history1.UserId,
            history1.FoodId,
            currentTime.AddHours(-1)
        );
        var historyReadModel2 = new FoodHistoryReadModel(
            history2.Id,
            history2.UserId,
            history2.FoodId,
            currentTime.AddHours(-2)
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
        result.First().Id.Should().Be(food1.Id);
        result.Last().Id.Should().Be(food2.Id);
    }

    [Fact]
    public async Task PrioritizeHistoryItemsAsync_ShouldOrderHistoryItemsFirst()
    {
        // Arrange
        var userId = UserId.NewId();
        var cancellationToken = CancellationToken.None;
        var currentTime = DateTime.UtcNow;
        var food1 = new FoodReadModel(
            FoodId.NewId(),
            "Food 1",
            "Description 1",
            "Category 1",
            "Brand 1"
        );
        var food2 = new FoodReadModel(
            FoodId.NewId(),
            "Food 2",
            "Description 2",
            "Category 2",
            "Brand 2"
        );
        var food3 = new FoodReadModel(
            FoodId.NewId(),
            "Food 3",
            "Description 3",
            "Category 3",
            "Brand 3"
        );
        var history1 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food1.Id).Value;
        var history2 = FoodHistory.Create(FoodHistoryId.NewId(), userId, food2.Id).Value;

        var historyReadModel1 = new FoodHistoryReadModel(
            history1.Id,
            history1.UserId,
            history1.FoodId,
            currentTime.AddHours(-1)
        );
        var historyReadModel2 = new FoodHistoryReadModel(
            history2.Id,
            history2.UserId,
            history2.FoodId,
            currentTime.AddHours(-2)
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
        result.First().Id.Should().Be(food1.Id);
        result.Skip(1).First().Id.Should().Be(food2.Id);
        result.Last().Id.Should().Be(food3.Id);
    }
}
