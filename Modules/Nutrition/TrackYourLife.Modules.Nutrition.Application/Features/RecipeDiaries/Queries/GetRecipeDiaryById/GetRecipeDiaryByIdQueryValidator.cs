using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;

public sealed class GetRecipeDiaryByIdQueryValidator : AbstractValidator<GetRecipeDiaryByIdQuery>
{
    public GetRecipeDiaryByIdQueryValidator()
    {
        RuleFor(x => x.DiaryId).NotEmptyId();
    }
}
