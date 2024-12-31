using System.Globalization;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.FoodDiaries.Queries;

public class GetTotalCaloriesByPeriodQueryHandlerTests : IDisposable
{
    private readonly IFoodDiaryQuery foodDiaryQuery = Substitute.For<IFoodDiaryQuery>();
    private readonly IUserIdentifierProvider userIdentifierProvider =
        Substitute.For<IUserIdentifierProvider>();

    private readonly GetTotalCaloriesByPeriodQueryHandler sut;

    public GetTotalCaloriesByPeriodQueryHandlerTests()
    {
        sut = new GetTotalCaloriesByPeriodQueryHandler(foodDiaryQuery, userIdentifierProvider);
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
    public async Task Handle_WhenFoodDiariesExits_ShouldReturnTotalCalories()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetTotalCaloriesByPeriodQuery(
            DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
            DateOnly.Parse("2022-01-05", CultureInfo.CurrentCulture)
        );

        List<FoodDiaryReadModel> foodDiaries = new List<FoodDiaryReadModel>
        {
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
                food: FoodFaker.GenerateReadModel(energyValue: 100),
                servingSize: ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 1f),
                quantity: 1
            ),
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
                food: FoodFaker.GenerateReadModel(energyValue: 20),
                servingSize: ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 0.5f),
                quantity: 2
            ),
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: DateOnly.Parse("2022-01-02", CultureInfo.CurrentCulture),
                food: FoodFaker.GenerateReadModel(energyValue: 5),
                servingSize: ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 1.5f),
                quantity: 10
            ),
        };

        foodDiaryQuery
            .GetFoodDiaryByPeriodQueryAsync(
                userId,
                DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
                DateOnly.Parse("2022-01-05", CultureInfo.CurrentCulture),
                Arg.Any<CancellationToken>()
            )
            .Returns(foodDiaries);

        userIdentifierProvider.UserId.Returns(userId);

        var total = 100 * 1 * 1 + 20 * 0.5 * 2 + 5 * 1.5 * 10;

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be((int)total);
    }

    [Fact]
    public async Task Handle_WhenNoFoodDiariesExits_ShouldReturnZeroTotalCalories()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetTotalCaloriesByPeriodQuery(
            DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
            DateOnly.Parse("2022-01-05", CultureInfo.CurrentCulture)
        );

        foodDiaryQuery
            .GetFoodDiaryByPeriodQueryAsync(
                userId,
                DateOnly.Parse("2022-01-01", CultureInfo.CurrentCulture),
                DateOnly.Parse("2022-01-05", CultureInfo.CurrentCulture),
                Arg.Any<CancellationToken>()
            )
            .Returns([]);

        userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }
}
