using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.ServingSizes;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Contracts.Common;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;

namespace TrackYourLife.Common.Application.FoodDiaries.Commands.AddFoodDiaryEntry;

public sealed class AddFoodDiaryEntryCommandHandler : ICommandHandler<AddFoodDiaryEntryCommand, IdResponse>
{
    public readonly IFoodDiaryRepository _foodDiaryEntryRepository;
    public readonly IFoodRepository _foodRepository;
    public readonly IServingSizeRepository _servingSizeRepository;
    public readonly IUnitOfWork _unitOfWork;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public AddFoodDiaryEntryCommandHandler(
        IFoodDiaryRepository foodDiaryEntryRepository,
        IUnitOfWork unitOfWork,
        IFoodRepository foodRepository,
        IServingSizeRepository servingSizeRepository,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        _foodDiaryEntryRepository = foodDiaryEntryRepository;
        _unitOfWork = unitOfWork;
        _foodRepository = foodRepository;
        _servingSizeRepository = servingSizeRepository;
        _userIdentifierProvider = userIdentifierProvider;
    }

    public async Task<Result<IdResponse>> Handle(
        AddFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        var food = await _foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<IdResponse>(DomainErrors.Food.NotFoundById(command.FoodId));
        }

        var servingSize = await _servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
        {
            return Result.Failure<IdResponse>(DomainErrors.ServingSize.NotFound(command.ServingSizeId));
        }

        var foodDiaryEntry = new FoodDiaryEntry(
            FoodDiaryEntryId.Create(Guid.NewGuid()),
            _userIdentifierProvider.UserId,
            food,
            command.Quantity,
            command.EntryDate,
            command.MealType,
            servingSize
        );

        await _foodDiaryEntryRepository.AddAsync(foodDiaryEntry, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(foodDiaryEntry.Id));
    }
}
