using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.UpdateUserGoal;

public sealed class UpdateUserGoalCommandHandler(
    IUserGoalRepository userGoalRepository,
    IUnitOfWork unitOfWork,
    IUserIdentifierProvider userIdentifierProvider
    ) : ICommandHandler<UpdateUserGoalCommand>
{
    private readonly IUserGoalRepository _userGoalRepository = userGoalRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserIdentifierProvider _userIdentifierProvider = userIdentifierProvider;

    public async Task<Result> Handle(
        UpdateUserGoalCommand command,
        CancellationToken cancellationToken
    )
    {
        UserGoal? newUserGoal = await _userGoalRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (newUserGoal is null)
        {
            return Result.Failure(UserGoalErrors.NotFound(command.Id));
        }

        if (newUserGoal.UserId != _userIdentifierProvider.UserId)
        {
            return Result.Failure(UserGoalErrors.NotOwned(command.Id));
        }

        newUserGoal.UpdateType(command.Type);
        newUserGoal.UpdatePerPeriod(command.PerPeriod);

        var result = Result.FirstFailureOrSuccess(
            newUserGoal.UpdateValue(command.Value),
            newUserGoal.UpdateStartDate(command.StartDate),
            newUserGoal.UpdateEndDate(command.EndDate ?? DateOnly.MaxValue)
        );

        if (result.IsFailure)
            return result;

        List<UserGoal> goals = (await _userGoalRepository.GetOverlappingGoalsAsync(
            newUserGoal,
            cancellationToken
        )).FindAll(g => g.Id != newUserGoal.Id).ToList();


        if (!command.Force && goals.Count > 0)
        {
            return Result.Failure(UserGoalErrors.Overlapping(command.Type));
        }

        foreach (var goal in goals)
        {
            if (goal.FullyOverlappedBy(newUserGoal))
            {
                _userGoalRepository.Remove(goal);
                continue;
            }

            if (goal.StartDate < newUserGoal.StartDate)
            {
                result = goal.UpdateEndDate(newUserGoal.StartDate.AddDays(-1));
                if (result.IsFailure)
                    return result;
                _userGoalRepository.Update(goal);
            }
            else //if (goal.EndDate > newUserGoal.EndDate)
            {
                result = goal.UpdateStartDate(newUserGoal.EndDate.AddDays(1));
                if (result.IsFailure)
                    return result;
                _userGoalRepository.Update(goal);
            }
            // else
            // {
            //     throw new InvalidOperationException("Goal is not fully overlapped by new goal");
            //     //!! Log this
            // }
        }

        _userGoalRepository.Update(newUserGoal);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
}
