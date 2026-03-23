using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetActiveWorkoutPlan;

public sealed class GetActiveWorkoutPlanQueryHandler(
    IWorkoutPlansQuery workoutPlansQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetActiveWorkoutPlanQuery, WorkoutPlanReadModel>
{
    public async Task<Result<WorkoutPlanReadModel>> Handle(
        GetActiveWorkoutPlanQuery request,
        CancellationToken cancellationToken
    )
    {
        var workoutPlan = await workoutPlansQuery.GetActiveByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (workoutPlan is null)
        {
            return Result.Failure<WorkoutPlanReadModel>(WorkoutPlanErrors.ActivePlanNotFound);
        }

        return Result.Success(workoutPlan);
    }
}
