using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;

public sealed class UpdateNutritionGoalsCommandValidator
    : AbstractValidator<UpdateNutritionGoalsCommand>
{
    public UpdateNutritionGoalsCommandValidator()
    {
        RuleFor(x => x.Calories).GreaterThan(0);
        RuleFor(x => x.Protein).GreaterThan(0);
        RuleFor(x => x.Carbohydrates).GreaterThan(0);
        RuleFor(x => x.Fats).GreaterThan(0);
        RuleFor(x => x.Force).NotNull();
    }
}
