using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;

public sealed class UpdateTrainingCommandHandler(
    ITrainingsRepository trainingsRepository,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider,
    IExerciseRepository exerciseRepository
) : ICommandHandler<UpdateTrainingCommand>
{
    public async Task<Result> Handle(
        UpdateTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var training = await trainingsRepository.GetByIdAsync(
            request.TrainingId,
            cancellationToken
        );

        if (training is null)
        {
            return Result.Failure(TrainingsErrors.NotFoundById(request.TrainingId));
        }

        if (training.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(TrainingsErrors.NotOwned(request.TrainingId));
        }

        training.UpdateDetails(
            request.Name,
            request.Duration,
            request.Description,
            dateTimeProvider.UtcNow
        );

        var exercises = await exerciseRepository.GetEnumerableWithinIdsCollectionAsync(
            request.ExerciseIds,
            cancellationToken
        );

        var trainingExercisesResults = exercises.Select(
            (exercise, index) => TrainingExercise.Create(training.Id, exercise, index)
        );

        var result = Result.FirstFailureOrSuccess([.. trainingExercisesResults]);

        if (result.IsFailure)
        {
            return Result.Failure<TrainingId>(result.Error);
        }

        training.UpdateExercises(
            trainingExercisesResults.Select(r => r.Value).ToList(),
            dateTimeProvider.UtcNow
        );

        trainingsRepository.Update(training);

        return Result.Success();
    }
}
