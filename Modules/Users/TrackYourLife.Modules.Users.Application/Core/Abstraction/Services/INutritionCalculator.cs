using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface INutritionCalculator
{
    public int CalculateCalories(
        int age,
        int weight,
        int height,
        Gender gender,
        ActivityLevel activityLevel,
        FitnessGoal FitnessGoal
    );
    int CalculateProtein(int weight);
    int CalculateCarbs(int calories);
    int CalculateFat(int calories);
}
