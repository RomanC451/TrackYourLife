using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.DailyNutritionOverviews;

public class DailyNutritionOverviewTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateDailyNutritionOverview()
    {
        // Arrange
        var id = DailyNutritionOverviewId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var caloriesGoal = 2000f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;
        var proteinGoal = 150f;

        // Act
        var overview = DailyNutritionOverview
            .Create(id, userId, date, caloriesGoal, carbohydratesGoal, fatGoal, proteinGoal)
            .Value;

        // Assert
        overview.Should().NotBeNull();
        overview.Id.Should().Be(id);
        overview.UserId.Should().Be(userId);
        overview.Date.Should().Be(date);
        overview.CaloriesGoal.Should().Be(caloriesGoal);
        overview.CarbohydratesGoal.Should().Be(carbohydratesGoal);
        overview.FatGoal.Should().Be(fatGoal);
        overview.ProteinGoal.Should().Be(proteinGoal);
        overview.NutritionalContent.Should().NotBeNull();
        overview.NutritionalContent.Energy.Unit.Should().Be("Kcal");
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var caloriesGoal = 2000f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;
        var proteinGoal = 150f;

        // Act
        var result = DailyNutritionOverview.Create(
            DailyNutritionOverviewId.Empty,
            userId,
            date,
            caloriesGoal,
            carbohydratesGoal,
            fatGoal,
            proteinGoal
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Id");
        result.Error.Code.Should().Contain(nameof(DailyNutritionOverview));
    }

    [Fact]
    public void Create_WithInvalidUserId_ShouldFail()
    {
        // Arrange
        var id = DailyNutritionOverviewId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var caloriesGoal = 2000f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;
        var proteinGoal = 150f;

        // Act
        var result = DailyNutritionOverview.Create(
            id,
            UserId.Empty,
            date,
            caloriesGoal,
            carbohydratesGoal,
            fatGoal,
            proteinGoal
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("UserId");
        result.Error.Code.Should().Contain(nameof(DailyNutritionOverview));
    }

    [Fact]
    public void Create_WithNegativeGoals_ShouldFail()
    {
        // Arrange
        var id = DailyNutritionOverviewId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var caloriesGoal = -2000f;
        var carbohydratesGoal = 250f;
        var fatGoal = 70f;
        var proteinGoal = 150f;

        // Act
        var result = DailyNutritionOverview.Create(
            id,
            userId,
            date,
            caloriesGoal,
            carbohydratesGoal,
            fatGoal,
            proteinGoal
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(DailyNutritionOverview)}.{nameof(caloriesGoal).ToCapitalCase()}.NotPositive"
            );
    }

    [Fact]
    public void AddNutritionalValues_ShouldUpdateNutritionalContent()
    {
        // Arrange
        var overview = CreateValidOverview();
        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 100, Unit = "Kcal" },
            Carbohydrates = 20,
            Fat = 5,
            Protein = 10,
        };
        var quantity = 2f;

        // Act
        overview.AddNutritionalValues(nutritionalContent, quantity);

        // Assert
        overview.NutritionalContent.Energy.Value.Should().Be(200);
        overview.NutritionalContent.Energy.Unit.Should().Be("Kcal");
        overview.NutritionalContent.Carbohydrates.Should().Be(40);
        overview.NutritionalContent.Fat.Should().Be(10);
        overview.NutritionalContent.Protein.Should().Be(20);
    }

    [Fact]
    public void SubtractNutritionalValues_ShouldUpdateNutritionalContent()
    {
        // Arrange
        var overview = CreateValidOverview();
        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 100, Unit = "Kcal" },
            Carbohydrates = 20,
            Fat = 5,
            Protein = 10,
        };
        var quantity = 2f;

        // Act
        overview.AddNutritionalValues(nutritionalContent, quantity);
        overview.SubtractNutritionalValues(nutritionalContent, quantity);

        // Assert
        overview.NutritionalContent.Energy.Value.Should().Be(0);
        overview.NutritionalContent.Energy.Unit.Should().Be("Kcal");
        overview.NutritionalContent.Carbohydrates.Should().Be(0);
        overview.NutritionalContent.Fat.Should().Be(0);
        overview.NutritionalContent.Protein.Should().Be(0);
    }

    [Fact]
    public void UpdateCaloriesGoal_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var overview = CreateValidOverview();
        var newCaloriesGoal = 2500f;

        // Act
        var result = overview.UpdateCaloriesGoal(newCaloriesGoal);

        // Assert
        result.IsSuccess.Should().BeTrue();
        overview.CaloriesGoal.Should().Be(newCaloriesGoal);
    }

    [Fact]
    public void UpdateCaloriesGoal_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var overview = CreateValidOverview();
        var newCaloriesGoal = -2500f;

        // Act
        var result = overview.UpdateCaloriesGoal(newCaloriesGoal);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("NewCaloriesGoal");
        result.Error.Code.Should().Contain(nameof(DailyNutritionOverview));
        overview.CaloriesGoal.Should().Be(2000f); // Original value should remain unchanged
    }

    [Fact]
    public void UpdateCarbohydratesGoal_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var overview = CreateValidOverview();
        var newCarbohydratesGoal = 300f;

        // Act
        var result = overview.UpdateCarbohydratesGoal(newCarbohydratesGoal);

        // Assert
        result.IsSuccess.Should().BeTrue();
        overview.CarbohydratesGoal.Should().Be(newCarbohydratesGoal);
    }

    [Fact]
    public void UpdateFatGoal_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var overview = CreateValidOverview();
        var newFatGoal = 80f;

        // Act
        var result = overview.UpdateFatGoal(newFatGoal);

        // Assert
        result.IsSuccess.Should().BeTrue();
        overview.FatGoal.Should().Be(newFatGoal);
    }

    [Fact]
    public void UpdateProteinGoal_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var overview = CreateValidOverview();
        var newProteinGoal = 180f;

        // Act
        var result = overview.UpdateProteinGoal(newProteinGoal);

        // Assert
        result.IsSuccess.Should().BeTrue();
        overview.ProteinGoal.Should().Be(newProteinGoal);
    }

    private static DailyNutritionOverview CreateValidOverview()
    {
        return DailyNutritionOverview
            .Create(
                DailyNutritionOverviewId.NewId(),
                UserId.NewId(),
                DateOnly.FromDateTime(DateTime.UtcNow),
                2000f,
                250f,
                70f,
                150f
            )
            .Value;
    }
}
