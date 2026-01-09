using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public sealed class AdjustExerciseSetsCommandHandler(
    IExercisesQuery exerciseQuery,
    IExercisesHistoriesRepository exercisesHistoriesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<AdjustExerciseSetsCommand>
{
    public async Task<Result> Handle(
        AdjustExerciseSetsCommand request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseQuery.GetByIdAsync(request.ExerciseId, cancellationToken);

        if (exercise is null)
        {
            return Result.Failure(ExercisesErrors.NotFoundById(request.ExerciseId));
        }

        // Check if an exercise history already exists for this exercise in this ongoing training
        var existingHistory =
            await exercisesHistoriesRepository.GetByOngoingTrainingIdAndExerciseIdAsync(
                request.OngoingTrainingId,
                request.ExerciseId,
                cancellationToken
            );

        if (existingHistory is not null)
        {
            // Update existing history
            var updateResult = existingHistory.UpdateStatusAndSets(
                oldExerciseSets: exercise.ExerciseSets,
                newExerciseSets: request.NewExerciseSets,
                status: ExerciseStatus.Completed
            );

            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            exercisesHistoriesRepository.Update(existingHistory);
        }
        else
        {
            // Create new history
            var exerciseHistory = ExerciseHistory.Create(
                id: ExerciseHistoryId.NewId(),
                ongoingTrainingId: request.OngoingTrainingId,
                exerciseId: request.ExerciseId,
                oldExerciseSets: exercise.ExerciseSets,
                newExerciseSets: request.NewExerciseSets,
                status: ExerciseStatus.Completed,
                createdOnUtc: dateTimeProvider.UtcNow
            );

            if (exerciseHistory.IsFailure)
            {
                return Result.Failure(exerciseHistory.Error);
            }

            await exercisesHistoriesRepository.AddAsync(exerciseHistory.Value, cancellationToken);
        }

        return Result.Success();
    }
}
