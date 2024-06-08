using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.ServingSizes;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.UpdateFoodDiaryEntry;

public sealed class UpdateFoodDiaryEntryCommandHandler
    : ICommandHandler<UpdateFoodDiaryEntryCommand>
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;

    private readonly IServingSizeRepository _servingSizeRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UpdateFoodDiaryEntryCommandHandler(
        IFoodDiaryRepository foodDiaryRepository,
        IUnitOfWork unitOfWork,
        IServingSizeRepository servingSizeRepository,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _foodDiaryRepository = foodDiaryRepository;
        _unitOfWork = unitOfWork;
        _servingSizeRepository = servingSizeRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result> Handle(
        UpdateFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        FoodDiaryEntry? foodDiaryEntry = await _foodDiaryRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (foodDiaryEntry is null)
        {
            return Result.Failure(DomainErrors.FoodDiaryEntry.NotFound(command.Id));
        }
        else if (foodDiaryEntry.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(
                DomainErrors.FoodDiaryEntry.NotCorrectUser(
                    command.Id,
                    _userIdentifierProvider.UserId
                )
            );
        }

        var newServingSize = await _servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (newServingSize is null)
        {
            return Result.Failure(DomainErrors.ServingSize.NotFound(command.ServingSizeId));
        }

        foodDiaryEntry.Quantity = command.Quantity;

        foodDiaryEntry.ServingSize = newServingSize;

        foodDiaryEntry.MealType = command.MealType;

        _foodDiaryRepository.Update(foodDiaryEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
