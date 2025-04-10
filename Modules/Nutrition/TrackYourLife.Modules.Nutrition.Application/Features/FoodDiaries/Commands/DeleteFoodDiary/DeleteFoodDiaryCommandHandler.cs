using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;

/// <summary>
/// Handles the command to remove a food diary entry.
/// </summary>
/// <remarks>
/// This handler is responsible for processing the <see cref="DeleteFoodDiaryCommand"/>,
/// which includes validating the existence of the food diary entry, ensuring that the
/// current user owns the entry, and then removing the entry from the database.
/// </remarks>
internal sealed class DeleteFoodDiaryCommandHandler(
    IFoodDiaryRepository foodDiaryEntryRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteFoodDiaryCommand>
{
    public async Task<Result> Handle(
        DeleteFoodDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        var foodDiary = await foodDiaryEntryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (foodDiary is null)
        {
            return Result.Failure(FoodDiaryErrors.NotFound(command.Id));
        }
        else if (foodDiary.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(FoodDiaryErrors.NotOwned(command.Id));
        }

        foodDiaryEntryRepository.Remove(foodDiary);

        return Result.Success();
    }
}
