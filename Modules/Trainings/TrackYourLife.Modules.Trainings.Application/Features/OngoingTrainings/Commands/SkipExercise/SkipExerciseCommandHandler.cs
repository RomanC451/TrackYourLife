using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.SkipExercise;

public sealed class SkipExerciseCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IExercisesHistoriesRepository exercisesHistoriesRepository,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<SkipExerciseCommand>
{
    public async Task<Result> Handle(
        SkipExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsRepository.GetByIdAsync(
            request.OngoingTrainingId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure(OngoingTrainingErrors.NotFoundById(request.OngoingTrainingId));
        }

        if (ongoingTraining.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(OngoingTrainingErrors.NotOwned(request.OngoingTrainingId));
        }

        if (ongoingTraining.IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(request.OngoingTrainingId));
        }

        var currentExercise = ongoingTraining.CurrentExercise;

        // Get all exercise histories BEFORE adding/updating to ensure consistent state
        var allExerciseHistories = await exercisesHistoriesQuery.GetByOngoingTrainingIdAsync(
            request.OngoingTrainingId,
            cancellationToken
        );

        var completedOrSkippedExerciseIds = allExerciseHistories
            .Select(eh => eh.ExerciseId)
            .ToHashSet();

        // Include the current exercise that we're about to skip
        completedOrSkippedExerciseIds.Add(currentExercise.Id);

        // Check if an exercise history already exists for this exercise in this ongoing training
        var existingHistory =
            await exercisesHistoriesRepository.GetByOngoingTrainingIdAndExerciseIdAsync(
                request.OngoingTrainingId,
                currentExercise.Id,
                cancellationToken
            );

        if (existingHistory is not null)
        {
            // Update existing history
            var updateResult = existingHistory.UpdateStatusAndSets(
                oldExerciseSets: currentExercise.ExerciseSets.ToList(),
                newExerciseSets: new List<ExerciseSet>(),
                status: ExerciseStatus.Skipped
            );

            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            exercisesHistoriesRepository.Update(existingHistory);
        }
        else
        {
            // Create new ExerciseHistory with Skipped status
            var exerciseHistory = ExerciseHistory.Create(
                id: ExerciseHistoryId.NewId(),
                ongoingTrainingId: request.OngoingTrainingId,
                exerciseId: currentExercise.Id,
                oldExerciseSets: currentExercise.ExerciseSets.ToList(),
                newExerciseSets: new List<ExerciseSet>(),
                status: ExerciseStatus.Skipped,
                createdOnUtc: dateTimeProvider.UtcNow
            );

            if (exerciseHistory.IsFailure)
            {
                return Result.Failure(exerciseHistory.Error);
            }

            await exercisesHistoriesRepository.AddAsync(exerciseHistory.Value, cancellationToken);
        }

        // Move to first incomplete exercise
        var skipResult = ongoingTraining.SkipExercise(completedOrSkippedExerciseIds);

        if (skipResult.IsFailure)
        {
            return Result.Failure(skipResult.Error);
        }

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
