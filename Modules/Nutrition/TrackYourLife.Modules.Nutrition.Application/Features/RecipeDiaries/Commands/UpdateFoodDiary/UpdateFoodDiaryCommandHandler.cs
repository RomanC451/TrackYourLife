using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateFoodDiary;

public sealed class UpdateRecipeDiaryCommandHandler(
    IRecipeDiaryRepository recipeDiaryRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateRecipeDiaryCommand>
{
    public async Task<Result> Handle(
        UpdateRecipeDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipeDiary = await recipeDiaryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (recipeDiary is null)
        {
            return Result.Failure(RecipeDiaryErrors.NotFound(command.Id));
        }

        if (recipeDiary.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(RecipeDiaryErrors.NotOwned(command.Id));
        }

        var result = Result.FirstFailureOrSuccess(
            recipeDiary.UpdateQuantity(command.Quantity),
            recipeDiary.UpdateMealType(command.MealType)
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        recipeDiaryRepository.Update(recipeDiary);

        return Result.Success();
    }
}
