using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

public class GetNutritionOverviewByPeriodQueryHandlerTests
{
    private readonly IFoodDiaryQuery _foodDiaryQuery;
    private readonly IRecipeDiaryQuery _recipeDiaryQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetNutritionOverviewByPeriodQueryHandler _handler;

    public GetNutritionOverviewByPeriodQueryHandlerTests()
    {
        _foodDiaryQuery = Substitute.For<IFoodDiaryQuery>();
        _recipeDiaryQuery = Substitute.For<IRecipeDiaryQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetNutritionOverviewByPeriodQueryHandler(
            _foodDiaryQuery,
            _recipeDiaryQuery,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenNoEntriesExist_ShouldReturnEmptyOverview()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddDays(7);
        var query = new GetNutritionOverviewByPeriodQuery(startDate, endDate);

        _userIdentifierProvider.UserId.Returns(userId);
        _foodDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<FoodDiaryReadModel>());
        _recipeDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<RecipeDiaryReadModel>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Energy.Value.Should().Be(0);
        result.Value.Protein.Should().Be(0);
        result.Value.Fat.Should().Be(0);
        result.Value.Carbohydrates.Should().Be(0);
        result.Value.Sodium.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WhenFoodDiaryEntriesExist_ShouldCalculateCorrectTotals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddDays(2);
        var query = new GetNutritionOverviewByPeriodQuery(startDate, endDate);

        var food = FoodFaker.GenerateReadModel(
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Protein = 10,
                Fat = 5,
                Carbohydrates = 15,
                Sodium = 2,
            }
        );

        var servingSize = ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 1.0f);

        var foodDiaries = new[]
        {
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate,
                food: food,
                servingSize: servingSize,
                quantity: 2.0f
            ),
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate.AddDays(1),
                food: food,
                servingSize: servingSize,
                quantity: 3.0f
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _foodDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(foodDiaries);
        _recipeDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<RecipeDiaryReadModel>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Energy.Value.Should().Be(500); // 100 * (2 + 3)
        result.Value.Protein.Should().Be(50); // 10 * (2 + 3)
        result.Value.Fat.Should().Be(25); // 5 * (2 + 3)
        result.Value.Carbohydrates.Should().Be(75); // 15 * (2 + 3)
        result.Value.Sodium.Should().Be(10); // 2 * (2 + 3)
    }

    [Fact]
    public async Task Handle_WhenRecipeDiaryEntriesExist_ShouldCalculateCorrectTotals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddDays(2);
        var query = new GetNutritionOverviewByPeriodQuery(startDate, endDate);

        var recipe = RecipeFaker.GenerateReadModel(userId: userId, portions: 2);

        recipe.NutritionalContents.AddNutritionalValues(
            new NutritionalContent
            {
                Energy = new Energy { Value = 200, Unit = "Kcal" },
                Protein = 20,
                Fat = 10,
                Carbohydrates = 30,
                Sodium = 4,
            }
        );
        var recipeDiaries = new[]
        {
            RecipeDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate,
                recipe: recipe,
                quantity: 2.0f
            ),
            RecipeDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate.AddDays(1),
                recipe: recipe,
                quantity: 3.0f
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _foodDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<FoodDiaryReadModel>());
        _recipeDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(recipeDiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Energy.Value.Should().Be(500); // (200 / 2) * (2 + 3)
        result.Value.Protein.Should().Be(50); // (20 / 2) * (2 + 3)
        result.Value.Fat.Should().Be(25); // (10 / 2) * (2 + 3)
        result.Value.Carbohydrates.Should().Be(75); // (30 / 2) * (2 + 3)
        result.Value.Sodium.Should().Be(10); // (4 / 2) * (2  + 3)
    }

    [Fact]
    public async Task Handle_WhenBothFoodAndRecipeEntriesExist_ShouldCalculateCombinedTotals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddDays(2);
        var query = new GetNutritionOverviewByPeriodQuery(startDate, endDate);

        var food = FoodFaker.GenerateReadModel(
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Protein = 10,
                Fat = 5,
                Carbohydrates = 15,
                Sodium = 2,
            }
        );

        var recipe = RecipeFaker.GenerateReadModel(
            userId: userId,
            portions: 2,
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 200, Unit = "Kcal" },
                Protein = 20,
                Fat = 10,
                Carbohydrates = 30,
                Sodium = 4,
            }
        );

        var foodDiaries = new[]
        {
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate,
                food: food,
                quantity: 2.0f,
                servingSize: ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 2.0f)
            ),
        };

        var recipeDiaries = new[]
        {
            RecipeDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate,
                recipe: recipe,
                quantity: 3.0f
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _foodDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(foodDiaries);
        _recipeDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(recipeDiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Energy.Value.Should().Be(700); // (100 * 2 * 2) + ((200 / 2) * 3)
        result.Value.Protein.Should().Be(70); // (10 * 2 * 2 ) + ((20 / 2) * 3)
        result.Value.Fat.Should().Be(35); // (5 * 2 * 2) + ((10 / 2) * 3)
        result.Value.Carbohydrates.Should().Be(105); // (15 * 2 * 2) + ((30 / 2) * 3)
        result.Value.Sodium.Should().Be(14); // (2 * 2 * 2) + ((4 / 2) * 3)
    }

    [Fact]
    public async Task Handle_WhenEntriesExistOutsideDateRange_ShouldNotIncludeThemInTotals()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = startDate.AddDays(2);
        var query = new GetNutritionOverviewByPeriodQuery(startDate, endDate);

        var food = FoodFaker.GenerateReadModel(
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Protein = 10,
                Fat = 5,
                Carbohydrates = 15,
                Sodium = 2,
            }
        );

        var servingSize = ServingSizeFaker.GenerateReadModel(nutritionMultiplier: 1.0f);

        var foodDiaries = new[]
        {
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate,
                food: food,
                servingSize: servingSize,
                quantity: 2.0f
            ),
            FoodDiaryFaker.GenerateReadModel(
                userId: userId,
                date: startDate.AddDays(-1),
                food: food,
                servingSize: servingSize,
                quantity: 3.0f
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _foodDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(foodDiaries.Where(fd => fd.Date >= startDate && fd.Date <= endDate));
        _recipeDiaryQuery
            .GetByPeriodAsync(userId, startDate, endDate, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<RecipeDiaryReadModel>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Energy.Value.Should().Be(200); // Only include the entry within range
        result.Value.Protein.Should().Be(20);
        result.Value.Fat.Should().Be(10);
        result.Value.Carbohydrates.Should().Be(30);
        result.Value.Sodium.Should().Be(4);
    }
}
