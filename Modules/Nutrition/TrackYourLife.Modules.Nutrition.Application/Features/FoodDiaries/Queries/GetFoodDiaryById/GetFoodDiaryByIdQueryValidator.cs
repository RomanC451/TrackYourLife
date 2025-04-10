using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;

public sealed class GetFoodDiaryByIdQueryValidator : AbstractValidator<GetFoodDiaryByIdQuery>
{
    public GetFoodDiaryByIdQueryValidator()
    {
        RuleFor(x => x.DiaryId).NotEmptyId();
    }
}
