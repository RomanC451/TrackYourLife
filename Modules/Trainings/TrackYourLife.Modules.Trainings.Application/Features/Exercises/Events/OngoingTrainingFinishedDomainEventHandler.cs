using Serilog;
using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings.Events;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Events;

internal sealed class OngoingTrainingFinishedDomainEventHandler(
    IExercisesRepository exercisesRepository,
    IExercisesHistoriesRepository exercisesHistoriesRepository,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    ILogger logger,
    ITrainingsUnitOfWork trainingsUnitOfWork
) : IDomainEventHandler<OngoingTrainingFinishedDomainEvent>
{
    public async Task Handle(
        OngoingTrainingFinishedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetByIdAsync(
            domainEvent.OngoingTrainingId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            logger.Error(
                "Failed to handle OngoingTrainingFinishedDomainEvent: OngoingTraining with id {OngoingTrainingId} not found",
                domainEvent.OngoingTrainingId
            );
            throw new EventFailedException(
                $"OngoingTraining with id {domainEvent.OngoingTrainingId} not found when handling OngoingTrainingFinishedDomainEvent"
            );
        }

        var exerciseHistories =
            await exercisesHistoriesRepository.GetByOngoingTrainingIdAndAreNotAppliedAsync(
                domainEvent.OngoingTrainingId,
                cancellationToken
            );

        foreach (var exerciseHistory in exerciseHistories)
        {
            var exercise = await exercisesRepository.GetByIdAsync(
                exerciseHistory.ExerciseId,
                cancellationToken
            );

            if (exercise is null)
            {
                logger.Error("Exercise with id {ExerciseId} not found", exerciseHistory.ExerciseId);
                continue;
            }

            var result = exercise.Update(
                exercise.Name,
                exercise.MuscleGroups,
                exercise.Difficulty,
                exercise.Description,
                exercise.VideoUrl,
                exercise.PictureUrl,
                exercise.Equipment,
                exerciseHistory.NewExerciseSets.ToList()
            );

            if (result.IsFailure)
            {
                logger.Error(
                    "Failed to update Exercise with id {ExerciseId}. Error: {Error}",
                    exercise.Id,
                    result.Error
                );
                continue;
            }

            exerciseHistory.SetAsChangesApplied();

            exercisesRepository.Update(exercise);

            exercisesHistoriesRepository.Update(exerciseHistory);
        }

        await trainingsUnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
