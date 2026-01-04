using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface INutritionGoalsCalculator
{
    public int CalculateCalories(
        int age,
        float weight,
        float height,
        Gender gender,
        ActivityLevel activityLevel,
        FitnessGoal FitnessGoal
    );
    int CalculateProtein(float weight);
    int CalculateCarbs(int calories);
    int CalculateFat(int calories);
}
