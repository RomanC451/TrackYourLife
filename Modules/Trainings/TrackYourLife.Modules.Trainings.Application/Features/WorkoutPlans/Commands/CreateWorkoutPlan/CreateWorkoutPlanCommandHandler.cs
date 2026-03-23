using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.CreateWorkoutPlan;

public sealed class CreateWorkoutPlanCommandHandler(
    IWorkoutPlansRepository workoutPlansRepository,
    IWorkoutPlansQuery workoutPlansQuery,
    ITrainingsQuery trainingsQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateWorkoutPlanCommand, WorkoutPlanId>
{
    public async Task<Result<WorkoutPlanId>> Handle(
        CreateWorkoutPlanCommand request,
        CancellationToken cancellationToken
    )
    {
        foreach (var trainingId in request.TrainingIds)
        {
            var training = await trainingsQuery.GetByIdAsync(trainingId, cancellationToken);
            if (training is null)
            {
                return Result.Failure<WorkoutPlanId>(TrainingsErrors.NotFoundById(trainingId));
            }

            if (training.UserId != userIdentifierProvider.UserId)
            {
                return Result.Failure<WorkoutPlanId>(TrainingsErrors.NotOwned(trainingId));
            }
        }

        var workoutPlans = await workoutPlansQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );
        var shouldBeActive = request.IsActive || !workoutPlans.Any();

        if (shouldBeActive)
        {
            var activePlan = await workoutPlansRepository.GetActiveByUserIdAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
            if (activePlan is not null)
            {
                activePlan.Deactivate();
                workoutPlansRepository.Update(activePlan);
            }
        }

        var workoutPlanId = WorkoutPlanId.NewId();
        var workoutPlanResult = WorkoutPlan.Create(
            workoutPlanId,
            userIdentifierProvider.UserId,
            request.Name,
            shouldBeActive,
            request.TrainingIds,
            dateTimeProvider.UtcNow
        );

        if (workoutPlanResult.IsFailure)
        {
            return Result.Failure<WorkoutPlanId>(workoutPlanResult.Error);
        }

        await workoutPlansRepository.AddAsync(workoutPlanResult.Value, cancellationToken);
        return Result.Success(workoutPlanId);
    }
}
