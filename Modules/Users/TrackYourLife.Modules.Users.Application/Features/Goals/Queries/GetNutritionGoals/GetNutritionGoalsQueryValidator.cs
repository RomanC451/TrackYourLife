using FluentValidation;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;

public sealed class GetNutritionGoalsQueryValidator : AbstractValidator<GetNutritionGoalsQuery>
{
    public GetNutritionGoalsQueryValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
    }
}
