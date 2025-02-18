using FluentValidation;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;

public sealed class CalculateNutritionGoalsCommandValidator
    : AbstractValidator<CalculateNutritionGoalsCommand>
{
    public CalculateNutritionGoalsCommandValidator()
    {
        RuleFor(x => x.Age).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThan(0);
        RuleFor(x => x.Height).GreaterThan(0);
        RuleFor(x => x.Gender).IsInEnum();
        RuleFor(x => x.ActivityLevel).IsInEnum();
        RuleFor(x => x.FitnessGoal).IsInEnum();
    }
}
