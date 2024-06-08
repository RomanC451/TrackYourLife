using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.RemoveFoodDiaryEntry;

public sealed class RemoveFoodDiaryEntryCommandHandler
    : ICommandHandler<RemoveFoodDiaryEntryCommand>
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public RemoveFoodDiaryEntryCommandHandler(
        IFoodDiaryRepository foodDiaryEntryRepository,
        IUnitOfWork unitOfWork,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _foodDiaryRepository = foodDiaryEntryRepository;
        _unitOfWork = unitOfWork;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result> Handle(
        RemoveFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        var foodDiaryEntry = await _foodDiaryRepository.GetByIdAsync(
            command.FoodDiaryEntryId,
            cancellationToken
        );

        if (foodDiaryEntry is null)
        {
            return Result.Failure(DomainErrors.FoodDiaryEntry.NotFound(command.FoodDiaryEntryId));
        }
        else if (foodDiaryEntry.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(
                DomainErrors.FoodDiaryEntry.NotCorrectUser(
                    command.FoodDiaryEntryId,
                    _userIdentifierProvider.UserId
                )
            );
        }

        _foodDiaryRepository.Remove(foodDiaryEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
