using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryByDate;
using TrackYourLife.Modules.Nutrition.Application.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Queries;

public class GetFoodDiaryByDateQueryHandlerTests : IDisposable
{
    private readonly IFoodDiaryQuery foodDiaryQuery = Substitute.For<IFoodDiaryQuery>();
    private readonly IUserIdentifierProvider userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();

    private readonly GetFoodDiaryByDateQueryHandler sut;

    public GetFoodDiaryByDateQueryHandlerTests()
    {
        sut = new GetFoodDiaryByDateQueryHandler(foodDiaryQuery, userIdentifierProvider);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodDiaryQuery.ClearSubstitute();
        userIdentifierProvider.ClearSubstitute();
    }

    [Fact]
    public async Task Handler_WhenEntriesDoesNotExistInDb_ReturnsEmptyResponse()
    {
        // Arrange
        var userId = UserId.NewId();

        var query = new GetFoodDiaryByDateQuery(DateOnly.FromDateTime(DateTime.UtcNow));

        foodDiaryQuery
            .GetFoodDiaryByDateQueryAsync(userId, query.Day, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handler_WhenEntriesExistInDb_ReturnsResponseWithCorrectData()
    {
        // Arrange
        var userId = UserId.NewId();

        var query = new GetFoodDiaryByDateQuery(DateOnly.FromDateTime(DateTime.UtcNow));

        var foodDiaryEntries = new List<FoodDiaryReadModel>
        {
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Breakfast),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Dinner),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Dinner),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Snacks),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Lunch),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Lunch),
            FoodDiaryFaker.GenerateReadModel(userId: userId, mealType: MealTypes.Lunch)
        };

        userIdentifierProvider.UserId.Returns(userId);

        foodDiaryQuery
            .GetFoodDiaryByDateQueryAsync(userId, query.Day, Arg.Any<CancellationToken>())
            .Returns(foodDiaryEntries);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.Should().HaveCount(4);
        result.Value[MealTypes.Breakfast].Should().HaveCount(1);
        result.Value[MealTypes.Dinner].Should().HaveCount(2);
        result.Value[MealTypes.Snacks].Should().HaveCount(1);
        result.Value[MealTypes.Lunch].Should().HaveCount(3);
    }
}
