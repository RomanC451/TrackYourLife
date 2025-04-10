using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;

public sealed class GetRecipesByUserIdQueryValidator : AbstractValidator<GetRecipesByUserIdQuery>
{
    public GetRecipesByUserIdQueryValidator()
    {
        // No validation rules needed as the query has no parameters
    }
}
