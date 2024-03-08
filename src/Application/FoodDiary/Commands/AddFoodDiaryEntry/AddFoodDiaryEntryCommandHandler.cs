using TrackYourLifeDotnet.Application.Abstractions.Messaging;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.FoodDiaries.Repositories;
using TrackYourLifeDotnet.Domain.Foods.Repositories;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Domain.Repositories;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users.Repositories;

namespace TrackYourLifeDotnet.Application.FoodDiary.Commands.AddFoodDieryEntry;

public sealed class AddFoodDiaryEntryCommandHandler
    : ICommandHandler<AddFoodDiaryEntryCommand, AddFoodDiaryEntryResult>
{
    public readonly IFoodDiaryRepository _foodDiaryEntryRepository;

    public readonly IFoodRepository _foodRepository;

    public readonly IServingSizeRepository _servingSizeRepository;

    public readonly IUserRepository _userRepository;
    public readonly IUnitOfWork _unitOfWork;

    public readonly IAuthService _authService;

    public AddFoodDiaryEntryCommandHandler(
        IFoodDiaryRepository foodDiaryEntryRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IFoodRepository foodRepository,
        IServingSizeRepository servingSizeRepository
    )
    {
        _foodDiaryEntryRepository = foodDiaryEntryRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _foodRepository = foodRepository;
        _servingSizeRepository = servingSizeRepository;
    }

    public async Task<Result<AddFoodDiaryEntryResult>> Handle(
        AddFoodDiaryEntryCommand command,
        CancellationToken cancellationToken
    )
    {
        var userIdResponse = _authService.GetUserIdFromJwtToken();

        if (userIdResponse.IsFailure)
        {
            return Result.Failure<AddFoodDiaryEntryResult>(userIdResponse.Error);
        }

        var userId = new UserId(userIdResponse.Value);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AddFoodDiaryEntryResult>(DomainErrors.User.NotFound(userId));
        }

        var food = await _foodRepository.GetByIdAsync(command.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<AddFoodDiaryEntryResult>(
                DomainErrors.Food.NotFoundById(command.FoodId)
            );
        }

        var servingSize = await _servingSizeRepository.GetByIdAsync(
            command.ServingSizeId,
            cancellationToken
        );

        if (servingSize is null)
        {
            return Result.Failure<AddFoodDiaryEntryResult>(
                DomainErrors.ServingSize.NotFound(command.ServingSizeId)
            );
        }

        var foodDiaryEntry = new FoodDiaryEntry(
            new FoodDiaryEntryId(Guid.NewGuid()),
            user.Id,
            food,
            command.Quantity,
            command.Date,
            command.MealType,
            servingSize
        );

        await _foodDiaryEntryRepository.AddAsync(foodDiaryEntry, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AddFoodDiaryEntryResult(foodDiaryEntry.Id));
    }
}
