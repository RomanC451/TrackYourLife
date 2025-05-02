using TrackYourLife.Modules.Users.Application.Features.Goals.Services;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Services;

public sealed class NutritionGoalsCalculatorTests
{
    private readonly NutritionGoalsCalculator _calculator;

    public NutritionGoalsCalculatorTests()
    {
        _calculator = new NutritionGoalsCalculator();
    }

    [Theory]
    [InlineData(
        30,
        70,
        175,
        Gender.Male,
        ActivityLevel.ModeratelyActive,
        FitnessGoal.Maintain,
        2637
    )]
    [InlineData(
        30,
        70,
        175,
        Gender.Male,
        ActivityLevel.ModeratelyActive,
        FitnessGoal.WeightLoss,
        2137
    )]
    [InlineData(
        30,
        70,
        175,
        Gender.Male,
        ActivityLevel.ModeratelyActive,
        FitnessGoal.MuscleGain,
        3137
    )]
    [InlineData(25, 60, 165, Gender.Female, ActivityLevel.Sedentary, FitnessGoal.Maintain, 1700)]
    [InlineData(25, 60, 165, Gender.Female, ActivityLevel.Sedentary, FitnessGoal.WeightLoss, 1200)]
    [InlineData(25, 60, 165, Gender.Female, ActivityLevel.Sedentary, FitnessGoal.MuscleGain, 2200)]
    [InlineData(40, 80, 180, Gender.Male, ActivityLevel.Active, FitnessGoal.Maintain, 3099)]
    [InlineData(40, 80, 180, Gender.Male, ActivityLevel.Active, FitnessGoal.WeightLoss, 2599)]
    [InlineData(40, 80, 180, Gender.Male, ActivityLevel.Active, FitnessGoal.MuscleGain, 3599)]
    public void CalculateCalories_WithValidInput_ReturnsExpectedValue(
        int age,
        int weight,
        int height,
        Gender gender,
        ActivityLevel activityLevel,
        FitnessGoal fitnessGoal,
        int expectedCalories
    )
    {
        // Act
        var result = _calculator.CalculateCalories(
            age,
            weight,
            height,
            gender,
            activityLevel,
            fitnessGoal
        );

        // Assert
        result.Should().Be(expectedCalories);
    }

    [Theory]
    [InlineData(70, 112)] // 70kg * 1.6 = 112g
    [InlineData(60, 96)] // 60kg * 1.6 = 96g
    [InlineData(80, 128)] // 80kg * 1.6 = 128g
    [InlineData(50, 80)] // 50kg * 1.6 = 80g
    public void CalculateProtein_WithValidInput_ReturnsExpectedValue(
        int weight,
        int expectedProtein
    )
    {
        // Act
        var result = _calculator.CalculateProtein(weight);

        // Assert
        result.Should().Be(expectedProtein);
    }

    [Theory]
    [InlineData(2000, 250)] // 2000 * 0.5 / 4 = 250g
    [InlineData(1500, 187)] // 1500 * 0.5 / 4 = 187.5g (rounded down)
    [InlineData(2500, 312)] // 2500 * 0.5 / 4 = 312.5g (rounded down)
    [InlineData(3000, 375)] // 3000 * 0.5 / 4 = 375g
    public void CalculateCarbs_WithValidInput_ReturnsExpectedValue(int calories, int expectedCarbs)
    {
        // Act
        var result = _calculator.CalculateCarbs(calories);

        // Assert
        result.Should().Be(expectedCarbs);
    }

    [Theory]
    [InlineData(2000, 66)] // 2000 * 0.3 / 9 = 66.67g (rounded down)
    [InlineData(1500, 50)] // 1500 * 0.3 / 9 = 50g
    [InlineData(2500, 83)] // 2500 * 0.3 / 9 = 83.33g (rounded down)
    [InlineData(3000, 100)] // 3000 * 0.3 / 9 = 100g
    public void CalculateFat_WithValidInput_ReturnsExpectedValue(int calories, int expectedFat)
    {
        // Act
        var result = _calculator.CalculateFat(calories);

        // Assert
        result.Should().Be(expectedFat);
    }
}
