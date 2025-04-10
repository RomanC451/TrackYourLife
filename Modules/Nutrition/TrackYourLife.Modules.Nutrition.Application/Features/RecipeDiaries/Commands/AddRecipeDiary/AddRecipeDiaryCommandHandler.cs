using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;

internal sealed class AddRecipeDiaryCommandHandler(
    IRecipeDiaryRepository recipeDiaryRepository,
    IRecipeRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<AddRecipeDiaryCommand, NutritionDiaryId>
{
    public async Task<Result<NutritionDiaryId>> Handle(
        AddRecipeDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipe = await recipeRepository.GetByIdAsync(command.RecipeId, cancellationToken);

        if (recipe is null)
        {
            return Result.Failure<NutritionDiaryId>(RecipeErrors.NotFound(command.RecipeId));
        }

        var result = RecipeDiary.Create(
            NutritionDiaryId.Create(Guid.NewGuid()),
            userIdentifierProvider.UserId,
            command.RecipeId,
            command.Quantity,
            command.EntryDate,
            command.MealType
        );

        if (result.IsFailure)
        {
            return Result.Failure<NutritionDiaryId>(result.Error);
        }

        await recipeDiaryRepository.AddAsync(result.Value, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
