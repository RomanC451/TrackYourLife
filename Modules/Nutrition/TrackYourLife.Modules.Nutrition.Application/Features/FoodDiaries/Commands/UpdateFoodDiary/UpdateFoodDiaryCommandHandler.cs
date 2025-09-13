using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;

///<summary>
/// Handles the command to update an existing food diary entry.
/// </summary>
/// <remarks>
/// This handler is responsible for processing the <see cref="UpdateFoodDiaryCommand"/>. It performs several checks to ensure
/// the integrity and validity of the update operation, including verifying the existence of the food diary entry, ensuring
/// the user making the request matches the entry's user, and validating the new serving size. If all validations pass, it
/// updates the food diary entry with the new details provided in the command.
/// </remarks>
/// <param name="foodDiaryRepository">The repository for accessing and updating food diary entities.</param>
/// <param name="servingSizeRepository">The repository for accessing serving size entities.</param>
/// <param name="userIdentifierProvider">The provider for identifying the current user.</param>
internal sealed class UpdateFoodDiaryCommandHandler(
    IFoodDiaryRepository foodDiaryRepository,
    IFoodRepository foodRepository,
    IServingSizeRepository servingSizeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateFoodDiaryCommand>
{
    public async Task<Result> Handle(
        UpdateFoodDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        FoodDiary? foodDiary = await foodDiaryRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (foodDiary is null)
        {
            return Result.Failure(FoodDiaryErrors.NotFound(command.Id));
        }

        if (foodDiary.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(FoodDiaryErrors.NotOwned(command.Id));
        }

        var newServingSize = await servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (newServingSize is null)
        {
            return Result.Failure(ServingSizeErrors.NotFound(command.ServingSizeId));
        }

        var food = await foodRepository.GetByIdAsync(foodDiary.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure(FoodErrors.NotFoundById(foodDiary.FoodId));
        }

        if (!food.HasServingSize(newServingSize.Id))
        {
            return Result.Failure(FoodErrors.ServingSizeNotFound(food.Id, command.ServingSizeId));
        }

        var result = Result.FirstFailureOrSuccess(
            foodDiary.UpdateQuantity(command.Quantity),
            foodDiary.UpdateServingSizeId(newServingSize.Id),
            foodDiary.UpdateMealType(command.MealType),
            foodDiary.UpdateEntryDate(command.EntryDate)
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        foodDiaryRepository.Update(foodDiary);

        return Result.Success();
    }
}
