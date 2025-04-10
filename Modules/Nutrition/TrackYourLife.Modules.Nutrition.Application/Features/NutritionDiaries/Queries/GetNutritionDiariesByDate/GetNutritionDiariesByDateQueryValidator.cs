using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

public sealed class GetNutritionDiariesByDateQueryValidator
    : AbstractValidator<GetNutritionDiariesByDateQuery>
{
    public GetNutritionDiariesByDateQueryValidator()
    {
        RuleFor(x => x.Day).NotEmpty();
    }
}
