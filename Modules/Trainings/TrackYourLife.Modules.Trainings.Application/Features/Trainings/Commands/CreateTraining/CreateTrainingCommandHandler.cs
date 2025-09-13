using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;

public sealed class CreateTrainingCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesRepository exercisesRepository,
    ITrainingsRepository trainingsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateTrainingCommand, TrainingId>
{
    public async Task<Result<TrainingId>> Handle(
        CreateTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var trainingId = TrainingId.NewId();

        var exercises = await exercisesRepository.GetEnumerableWithinIdsCollectionAsync(
            request.ExercisesIds,
            cancellationToken
        );

        if (exercises.Count() != request.ExercisesIds.Count)
        {
            var missingExercisesIds = request.ExercisesIds.Except(exercises.Select(e => e.Id));

            return Result.Failure<TrainingId>(
                ExercisesErrors.NotFoundById(missingExercisesIds.First())
            );
        }

        var trainingExercisesResults = request.ExercisesIds.Select(
            (id, index) =>
                TrainingExercise.Create(trainingId, exercises.First(e => e.Id == id), index)
        );

        var result = Result.FirstFailureOrSuccess([.. trainingExercisesResults]);

        if (result.IsFailure)
        {
            return Result.Failure<TrainingId>(result.Error);
        }

        var training = Training.Create(
            id: trainingId,
            userId: userIdentifierProvider.UserId,
            name: request.Name,
            muscleGroups: request.MuscleGroups,
            difficulty: request.Difficulty,
            trainingExercises: trainingExercisesResults.Select(r => r.Value).ToList(),
            createdOnUtc: dateTimeProvider.UtcNow,
            duration: request.Duration,
            restSeconds: request.RestSeconds,
            description: request.Description
        );

        if (training.IsFailure)
        {
            return Result.Failure<TrainingId>(training.Error);
        }

        await trainingsRepository.AddAsync(training.Value, cancellationToken);

        return Result.Success(training.Value.Id);
    }
}
