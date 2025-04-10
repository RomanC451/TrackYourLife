using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;

internal sealed class DeleteRecipeDiaryCommandHandler(
    IRecipeDiaryRepository recipeDiaryRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteRecipeDiaryCommand>
{
    public async Task<Result> Handle(
        DeleteRecipeDiaryCommand command,
        CancellationToken cancellationToken
    )
    {
        var recipeDiary = await recipeDiaryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (recipeDiary is null)
        {
            return Result.Failure(RecipeDiaryErrors.NotFound(command.Id));
        }
        else if (recipeDiary.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(RecipeDiaryErrors.NotOwned(command.Id));
        }

        recipeDiaryRepository.Remove(recipeDiary);

        return Result.Success();
    }
}
