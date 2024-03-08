using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.FoodDiaries.Repositories;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.UpdateFoodDiaryEntry;

public sealed class UpdateFoodDiaryEntryCommandHandler
    : ICommandHandler<UpdateFoodDiaryEntryCommand>
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;

    private readonly IServingSizeRepository _servingSizeRepository;

    private readonly IUnitOfWork _unitOfWork;

    public UpdateFoodDiaryEntryCommandHandler(
        IFoodDiaryRepository foodDiaryRepository,
        IUnitOfWork unitOfWork,
        IServingSizeRepository servingSizeRepository
    )
    {
        _foodDiaryRepository = foodDiaryRepository;
        _unitOfWork = unitOfWork;
        _servingSizeRepository = servingSizeRepository;
    }

    public async Task<Result> Handle(
        UpdateFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        FoodDiaryEntry? foodDiaryEntry = await _foodDiaryRepository.GetByIdAsync(
            command.FoodDiaryEntryId,
            cancellationToken
        );

        if (foodDiaryEntry is null)
        {
            return Result.Failure(DomainErrors.FoodDiaryEntry.NotFound(command.FoodDiaryEntryId));
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
