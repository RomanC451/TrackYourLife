using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommandHandler(
    IExerciseRepository exerciseRepository,
    ITrainingsRepository trainingsRepository,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<DeleteExerciseCommand>
{
    public async Task<Result> Handle(
        DeleteExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseRepository.GetByIdAsync(request.ExerciseId, cancellationToken);

        if (exercise is null)
        {
            return Result.Failure(ExercisesErrors.NotFoundById(request.ExerciseId));
        }

        if (exercise.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(ExercisesErrors.NotFoundById(request.ExerciseId));
        }

        var trainings = await trainingsRepository.GetThatContainsExerciseAsync(
            request.ExerciseId,
            cancellationToken
        );

        if (trainings.Any())
        {
            if (!request.ForceDelete)
            {
                return Result.Failure(ExercisesErrors.UsedInTrainings(request.ExerciseId));
            }

            foreach (var training in trainings)
            {
                training.RemoveExercise(request.ExerciseId, dateTimeProvider.UtcNow);
            }

            trainingsRepository.UpdateRange(trainings);
        }

        exerciseRepository.Remove(exercise);

        return Result.Success();
    }
}
