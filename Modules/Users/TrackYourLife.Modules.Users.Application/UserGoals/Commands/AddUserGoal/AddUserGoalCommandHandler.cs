using MediatR;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Contracts.UserGoals;
using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.AddUserGoal;

public sealed class UpdateUserGoalCommandHandler(
    IUserGoalRepository userGoalRepository,
    IUnitOfWork unitOfWork,
    IUserIdentifierProvider userIdentifierProvider
    ) : ICommandHandler<AddUserGoalCommand, AddUserGoalResponse>
{
    private readonly IUserGoalRepository _userGoalRepository = userGoalRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserIdentifierProvider _userIdentifierProvider = userIdentifierProvider;

    public async Task<Result<AddUserGoalResponse>> Handle(
        AddUserGoalCommand command,
        CancellationToken cancellationToken
    )
    {
        var userGoalResult = UserGoal.Create(
            id: UserGoalId.NewId(),
            userId: _userIdentifierProvider.UserId,
            type: command.Type,
            value: command.Value,
            perPeriod: command.PerPeriod,
            startDate: command.StartDate,
            endDate: command.EndDate
        );

        if (userGoalResult.IsFailure)
            return Result.Failure<AddUserGoalResponse>(userGoalResult.Error);

        var newUserGoal = userGoalResult.Value;


        List<UserGoal> goals = await _userGoalRepository.GetOverlappingGoalsAsync(
            newUserGoal,
            cancellationToken
        );

        if (!command.Force && goals.Count > 0)
            return Result.Failure<AddUserGoalResponse>(UserGoalErrors.Overlapping(command.Type));


        Result result;

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
                    return Result.Failure<AddUserGoalResponse>(result.Error);
                _userGoalRepository.Update(goal);
            }
            else
            {
                result = goal.UpdateStartDate(newUserGoal.EndDate.AddDays(1));
                if (result.IsFailure)
                    return Result.Failure<AddUserGoalResponse>(result.Error);
                _userGoalRepository.Update(goal);
            }
            // else
            // {
            //     throw new InvalidOperationException("Goal is not fully overlapped by new goal");
            //     //!! Log this
            // }
        }

        await _userGoalRepository.AddAsync(newUserGoal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(new AddUserGoalResponse(newUserGoal.Id));
    }

}
