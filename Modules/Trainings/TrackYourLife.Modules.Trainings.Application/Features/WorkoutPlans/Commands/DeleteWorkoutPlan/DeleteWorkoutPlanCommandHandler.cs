using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;

public sealed class DeleteWorkoutPlanCommandHandler(
    IWorkoutPlansRepository workoutPlansRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteWorkoutPlanCommand>
{
    public async Task<Result> Handle(
        DeleteWorkoutPlanCommand request,
        CancellationToken cancellationToken
    )
    {
        var workoutPlan = await workoutPlansRepository.GetByIdAsync(
            request.WorkoutPlanId,
            cancellationToken
        );
        if (workoutPlan is null)
        {
            return Result.Failure(WorkoutPlanErrors.NotFoundById(request.WorkoutPlanId));
        }

        if (workoutPlan.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(WorkoutPlanErrors.NotOwned(request.WorkoutPlanId));
        }

        workoutPlansRepository.Remove(workoutPlan);
        return Result.Success();
    }
}
