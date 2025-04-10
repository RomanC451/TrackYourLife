using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;

public class NutritionCalculator : INutritionCalculator
{
    public int CalculateCalories(
        int age,
        int weight,
        int height,
        Gender gender,
        ActivityLevel activityLevel,
        FitnessGoal FitnessGoal
    )
    {
        // Calculate BMR
        double bmr =
            gender == Gender.Male
                ? 66.47 + (13.75 * weight) + (5.003 * height) - (6.755 * age)
                : 655.1 + (9.563 * weight) + (1.850 * height) - (4.676 * age);

        // Calculate AMR
        double activityMultiplier = activityLevel switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.LightlyActive => 1.375,
            ActivityLevel.ModeratelyActive => 1.55,
            ActivityLevel.Active => 1.725,
            ActivityLevel.ExtremelyActive => 1.9,
            _ => 1.2
        };

        int amr = (int)(bmr * activityMultiplier);

        // Adjust calories based on goal
        return FitnessGoal switch
        {
            FitnessGoal.WeightLoss => amr - 500,
            FitnessGoal.MuscleGain => amr + 500,
            _ => amr
        };
    }

    public int CalculateProtein(int weight)
    {
        // Example calculation logic for protein
        return (int)(weight * 1.6);
    }

    public int CalculateCarbs(int calories)
    {
        // Example calculation logic for carbs
        return (int)(calories * 0.5 / 4);
    }

    public int CalculateFat(int calories)
    {
        // Example calculation logic for fat
        return (int)(calories * 0.3 / 9);
    }
}
