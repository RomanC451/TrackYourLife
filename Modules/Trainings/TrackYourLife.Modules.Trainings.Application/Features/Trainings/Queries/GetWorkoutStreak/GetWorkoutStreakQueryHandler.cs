using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutStreak;

public class GetWorkoutStreakQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IOngoingTrainingsQuery ongoingTrainingsQuery
) : IQueryHandler<GetWorkoutStreakQuery, WorkoutStreakDto>
{
    public async Task<Result<WorkoutStreakDto>> Handle(
        GetWorkoutStreakQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var completedWorkouts = await ongoingTrainingsQuery.GetCompletedByUserIdAsync(
            userId,
            cancellationToken
        );

        var workoutDays = completedWorkouts
            .Where(w => w.FinishedOnUtc.HasValue)
            .Select(w => DateOnly.FromDateTime(w.FinishedOnUtc!.Value))
            .ToHashSet();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);

        if (!workoutDays.Contains(today) && !workoutDays.Contains(yesterday))
        {
            return Result.Success(new WorkoutStreakDto(0));
        }

        var currentDate = workoutDays.Contains(today) ? today : yesterday;
        var dayStreak = 0;

        while (workoutDays.Contains(currentDate))
        {
            dayStreak++;
            currentDate = currentDate.AddDays(-1);
        }

        return Result.Success(new WorkoutStreakDto(dayStreak));
    }
}
