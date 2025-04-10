using FluentValidation;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;

public sealed class GetFoodByIdQueryValidator : AbstractValidator<GetFoodByIdQuery>
{
    public GetFoodByIdQueryValidator()
    {
        RuleFor(x => x.FoodId).NotNull();
    }
}
