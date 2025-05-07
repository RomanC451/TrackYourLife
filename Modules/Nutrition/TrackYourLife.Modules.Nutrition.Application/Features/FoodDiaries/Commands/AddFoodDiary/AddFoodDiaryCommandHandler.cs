using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;

/// <summary>
/// Handles the addition of a new food diary entry.
/// </summary>
/// <remarks>
/// This handler is responsible for processing the <see cref="AddFoodDiaryCommand"/>. It performs several steps to ensure
/// the integrity of the operation, including validating the existence of the specified food and serving size, ensuring the
/// serving size is applicable to the food, and creating a new food diary entry with the provided details. If any step fails,
/// it returns an error; otherwise, it successfully adds the new entry to the food diary database.
/// </remarks>
/// <param name="foodDiaryEntryRepository">The repository for accessing and adding food diary entries.</param>
/// <param name="foodRepository">The repository for accessing food items.</param>
/// <param name="servingSizeRepository">The repository for accessing serving sizes.</param>
/// <param name="userIdentifierProvider">The provider for identifying the current user.</param>
internal sealed class AddFoodDiaryCommandHandler(
    IFoodDiaryRepository foodDiaryEntryRepository,
    IFoodRepository foodRepository,
    IServingSizeRepository servingSizeRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<AddFoodDiaryCommand, NutritionDiaryId>
{
    public async Task<Result<NutritionDiaryId>> Handle(
        AddFoodDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        var food = await foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<NutritionDiaryId>(FoodErrors.NotFoundById(command.FoodId));
        }

        var servingSize = await servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
        {
            return Result.Failure<NutritionDiaryId>(
                ServingSizeErrors.NotFound(command.ServingSizeId)
            );
        }

        if (!food.HasServingSize(servingSize.Id))
        {
            return Result.Failure<NutritionDiaryId>(
                FoodErrors.ServingSizeNotFound(food.Id, servingSize.Id)
            );
        }

        var result = FoodDiary.Create(
            NutritionDiaryId.Create(Guid.NewGuid()),
            userIdentifierProvider.UserId,
            food.Id,
            command.Quantity,
            command.EntryDate,
            command.MealType,
            servingSize.Id
        );

        if (result.IsFailure)
        {
            return Result.Failure<NutritionDiaryId>(result.Error);
        }

        await foodDiaryEntryRepository.AddAsync(result.Value, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
