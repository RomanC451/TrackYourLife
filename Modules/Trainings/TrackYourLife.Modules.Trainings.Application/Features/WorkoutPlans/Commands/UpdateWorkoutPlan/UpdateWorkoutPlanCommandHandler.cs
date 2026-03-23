using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.UpdateWorkoutPlan;

public sealed class UpdateWorkoutPlanCommandHandler(
    IWorkoutPlansRepository workoutPlansRepository,
    ITrainingsQuery trainingsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateWorkoutPlanCommand>
{
    public async Task<Result> Handle(
        UpdateWorkoutPlanCommand request,
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

        foreach (var trainingId in request.TrainingIds)
        {
            var training = await trainingsQuery.GetByIdAsync(trainingId, cancellationToken);
            if (training is null)
            {
                return Result.Failure(TrainingsErrors.NotFoundById(trainingId));
            }

            if (training.UserId != userIdentifierProvider.UserId)
            {
                return Result.Failure(TrainingsErrors.NotOwned(trainingId));
            }
        }

        if (request.IsActive)
        {
            var activePlan = await workoutPlansRepository.GetActiveByUserIdAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
            if (activePlan is not null && activePlan.Id != workoutPlan.Id)
            {
                activePlan.Deactivate();
                workoutPlansRepository.Update(activePlan);
            }
        }

        var detailsUpdateResult = workoutPlan.UpdateDetails(request.Name, request.IsActive);
        if (detailsUpdateResult.IsFailure)
        {
            return Result.Failure(detailsUpdateResult.Error);
        }

        var trainingsUpdateResult = workoutPlan.ReplaceTrainings(request.TrainingIds);
        if (trainingsUpdateResult.IsFailure)
        {
            return Result.Failure(trainingsUpdateResult.Error);
        }

        workoutPlansRepository.Update(workoutPlan);
        return Result.Success();
    }
}
