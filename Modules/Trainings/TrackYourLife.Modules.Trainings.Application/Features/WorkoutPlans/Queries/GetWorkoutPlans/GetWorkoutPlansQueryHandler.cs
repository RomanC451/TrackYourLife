using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Queries.GetWorkoutPlans;

public sealed class GetWorkoutPlansQueryHandler(
    IWorkoutPlansQuery workoutPlansQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetWorkoutPlansQuery, IEnumerable<WorkoutPlanReadModel>>
{
    public async Task<Result<IEnumerable<WorkoutPlanReadModel>>> Handle(
        GetWorkoutPlansQuery request,
        CancellationToken cancellationToken
    )
    {
        var workoutPlans = await workoutPlansQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(workoutPlans);
    }
}
