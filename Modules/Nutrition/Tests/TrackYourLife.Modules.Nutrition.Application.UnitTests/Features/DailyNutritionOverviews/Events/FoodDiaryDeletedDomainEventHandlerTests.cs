using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries.Events;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Events;

public class FoodDiaryDeletedDomainEventHandlerTests
{
    private readonly UserId _userId;
    private readonly DateOnly _date;
    private readonly FoodId _foodId;
    private readonly ServingSizeId _servingSizeId;

    public FoodDiaryDeletedDomainEventHandlerTests()
    {
        _userId = UserId.NewId();
        _date = DateOnly.FromDateTime(DateTime.Today);
        _foodId = FoodId.NewId();
        _servingSizeId = ServingSizeId.NewId();
    }

    [Fact]
    public async Task Handle_WhenFoodNotFound_ShouldNotUpdateOverview()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        foodQuery.GetByIdAsync(_foodId, default).Returns((FoodReadModel?)null);

        var handler = new FoodDiaryDeletedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork
        );

        var domainEvent = new FoodDiaryDeletedDomainEvent(
            _userId,
            _foodId,
            _servingSizeId,
            _date,
            1
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenServingSizeNotFound_ShouldNotUpdateOverview()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var food = FoodFaker.GenerateReadModel(
            id: _foodId,
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Calcium = 0,
                Carbohydrates = 20,
                Cholesterol = 0,
                Fat = 5,
                Fiber = 0,
                Iron = 0,
                MonounsaturatedFat = 0,
                NetCarbs = 0,
                PolyunsaturatedFat = 0,
                Potassium = 0,
                Protein = 2,
                SaturatedFat = 0,
                Sodium = 0,
                Sugar = 0,
                TransFat = 0,
                VitaminA = 0,
                VitaminC = 0,
            }
        );

        foodQuery.GetByIdAsync(_foodId, default).Returns(food);
        servingSizeQuery.GetByIdAsync(_servingSizeId, default).Returns((ServingSizeReadModel?)null);

        var handler = new FoodDiaryDeletedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork
        );

        var domainEvent = new FoodDiaryDeletedDomainEvent(
            _userId,
            _foodId,
            _servingSizeId,
            _date,
            1
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenOverviewNotFound_ShouldNotUpdateOverview()
    {
        // Arrange
        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var food = FoodFaker.GenerateReadModel(
            id: _foodId,
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Calcium = 0,
                Carbohydrates = 20,
                Cholesterol = 0,
                Fat = 5,
                Fiber = 0,
                Iron = 0,
                MonounsaturatedFat = 0,
                NetCarbs = 0,
                PolyunsaturatedFat = 0,
                Potassium = 0,
                Protein = 2,
                SaturatedFat = 0,
                Sodium = 0,
                Sugar = 0,
                TransFat = 0,
                VitaminA = 0,
                VitaminC = 0,
            }
        );

        var servingSize = ServingSizeFaker.GenerateReadModel(
            id: _servingSizeId,
            nutritionMultiplier: 1.0f,
            unit: "g",
            value: 100.0f
        );

        foodQuery.GetByIdAsync(_foodId, default).Returns(food);
        servingSizeQuery.GetByIdAsync(_servingSizeId, default).Returns(servingSize);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns((DailyNutritionOverview?)null);

        var handler = new FoodDiaryDeletedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork
        );

        var domainEvent = new FoodDiaryDeletedDomainEvent(
            _userId,
            _foodId,
            _servingSizeId,
            _date,
            1
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.DidNotReceive().Update(Arg.Any<DailyNutritionOverview>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_WhenAllDataFound_ShouldUpdateOverview()
    {
        // Arrange
        DailyNutritionOverview updatedOverview = null!;

        var foodQuery = Substitute.For<IFoodQuery>();
        var servingSizeQuery = Substitute.For<IServingSizeQuery>();
        var dailyNutritionOverviewRepository = Substitute.For<IDailyNutritionOverviewRepository>();
        var unitOfWork = Substitute.For<INutritionUnitOfWork>();

        var food = FoodFaker.GenerateReadModel(
            id: _foodId,
            nutritionalContent: new NutritionalContent
            {
                Energy = new Energy { Value = 100, Unit = "Kcal" },
                Calcium = 0,
                Carbohydrates = 20,
                Cholesterol = 0,
                Fat = 5,
                Fiber = 0,
                Iron = 0,
                MonounsaturatedFat = 0,
                NetCarbs = 0,
                PolyunsaturatedFat = 0,
                Potassium = 0,
                Protein = 2,
                SaturatedFat = 0,
                Sodium = 0,
                Sugar = 0,
                TransFat = 0,
                VitaminA = 0,
                VitaminC = 0,
            }
        );

        var servingSize = ServingSizeFaker.GenerateReadModel(
            id: _servingSizeId,
            nutritionMultiplier: 1.5f,
            unit: "g",
            value: 100.0f
        );

        var overview = DailyNutritionOverviewFaker.Generate(
            userId: _userId,
            date: _date,
            caloriesGoal: 2000,
            carbohydratesGoal: 200,
            fatGoal: 50,
            proteinGoal: 100
        );

        overview.NutritionalContent.Energy.Value = 2000;
        overview.NutritionalContent.Carbohydrates = 200;
        overview.NutritionalContent.Fat = 50;
        overview.NutritionalContent.Protein = 100;

        foodQuery.GetByIdAsync(_foodId, default).Returns(food);
        servingSizeQuery.GetByIdAsync(_servingSizeId, default).Returns(servingSize);
        dailyNutritionOverviewRepository
            .GetByUserIdAndDateAsync(_userId, _date, default)
            .Returns(overview);

        dailyNutritionOverviewRepository
            .WhenForAnyArgs(x => x.Update(Arg.Any<DailyNutritionOverview>()))
            .Do(x => updatedOverview = x.Arg<DailyNutritionOverview>());

        var handler = new FoodDiaryDeletedDomainEventHandler(
            foodQuery,
            servingSizeQuery,
            dailyNutritionOverviewRepository,
            unitOfWork
        );

        var domainEvent = new FoodDiaryDeletedDomainEvent(
            _userId,
            _foodId,
            _servingSizeId,
            _date,
            2
        );

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        dailyNutritionOverviewRepository.Received(1).Update(updatedOverview);
        await unitOfWork.Received(1).SaveChangesAsync(default);

        updatedOverview.Should().NotBeNull();
        updatedOverview.Id.Should().Be(overview.Id);
        updatedOverview.UserId.Should().Be(overview.UserId);
        updatedOverview.Date.Should().Be(overview.Date);
        updatedOverview.CaloriesGoal.Should().Be(overview.CaloriesGoal);
        updatedOverview.CarbohydratesGoal.Should().Be(overview.CarbohydratesGoal);
        updatedOverview.FatGoal.Should().Be(overview.FatGoal);
        updatedOverview.ProteinGoal.Should().Be(overview.ProteinGoal);

        var expectedCalories = 2000 - (100 * 2 * 1.5f);
        var expectedCarbohydrates = 200 - (20 * 2 * 1.5f);
        var expectedFats = 50 - (5 * 2 * 1.5f);
        var expectedProtein = 100 - (2 * 2 * 1.5f);

        updatedOverview.NutritionalContent.Energy.Value.Should().Be(expectedCalories);
        updatedOverview.NutritionalContent.Carbohydrates.Should().Be(expectedCarbohydrates);
        updatedOverview.NutritionalContent.Fat.Should().Be(expectedFats);
        updatedOverview.NutritionalContent.Protein.Should().Be(expectedProtein);
    }
}
