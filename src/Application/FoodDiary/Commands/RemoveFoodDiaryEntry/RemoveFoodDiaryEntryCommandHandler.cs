using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.FoodDiaries.Repositories;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.RemoveFoodDiaryEntry;

public sealed class RemoveFoodDiaryEntryCommandHandler
    : ICommandHandler<RemoveFoodDiaryEntryCommand>
{
    private readonly IFoodDiaryRepository _foodDiaryRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly IAuthService _authService;

    public RemoveFoodDiaryEntryCommandHandler(
        IAuthService authService,
        IFoodDiaryRepository foodDiaryEntryRepository,
        IUnitOfWork unitOfWork
    )
    {
        _authService = authService;
        _foodDiaryRepository = foodDiaryEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        Result<Guid> userIdResult = _authService.GetUserIdFromJwtToken();

        if (userIdResult.IsFailure)
        {
            return Result.Failure(userIdResult.Error);
        }

        UserId userId = new(userIdResult.Value);

        var foodDiaryEntry = await _foodDiaryRepository.GetByIdAsync(
            command.FoodDiaryEntryId,
            cancellationToken
        );

        if (foodDiaryEntry is null)
        {
            return Result.Failure(DomainErrors.FoodDiaryEntry.NotFound(command.FoodDiaryEntryId));
        }
        else if (foodDiaryEntry.UserId != userId)
        {
            return Result.Failure(
                DomainErrors.FoodDiaryEntry.NotCorrectUser(command.FoodDiaryEntryId, userId)
            );
        }

        _foodDiaryRepository.Remove(foodDiaryEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
