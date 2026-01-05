using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.DailyNutritionOverviews.Models;

public class DailyNutritionOverviewMappingsExtensionsTests
{
    [Fact]
    public void ToDto_WithDailyNutritionOverviewReadModel_ShouldMapCorrectly()
    {
        // Arrange
        var overviewId = DailyNutritionOverviewId.NewId();
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 2000.0f, Unit = "Kcal" },
            Carbohydrates = 250.0f,
            Fat = 65.0f,
            Protein = 150.0f,
        };

        var overviewReadModel = new DailyNutritionOverviewReadModel(
            overviewId,
            userId,
            date,
            2000.0f,
            250.0f,
            65.0f,
            150.0f
        )
        {
            NutritionalContent = nutritionalContent,
        };

        // Act
        var dto = overviewReadModel.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(overviewId);
        dto.StartDate.Should().Be(date);
        dto.EndDate.Should().Be(date);
        dto.CaloriesGoal.Should().Be(2000.0f);
        dto.CarbohydratesGoal.Should().Be(250.0f);
        dto.FatGoal.Should().Be(65.0f);
        dto.ProteinGoal.Should().Be(150.0f);
        dto.NutritionalContent.Should().NotBeNull();
    }
}
