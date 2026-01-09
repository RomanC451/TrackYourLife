using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public sealed class FinishOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<FinishOngoingTrainingCommand>
{
    public async Task<Result> Handle(
        FinishOngoingTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsRepository.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure(OngoingTrainingErrors.NotFoundById(request.Id));
        }

        if (ongoingTraining.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(OngoingTrainingErrors.NotOwned(request.Id));
        }

        if (ongoingTraining.IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(request.Id));
        }

        // Validate that all exercises are completed or skipped
        var allExerciseIds = ongoingTraining.GetAllExerciseIds();
        var exerciseHistories = await exercisesHistoriesQuery.GetByOngoingTrainingIdAsync(
            request.Id,
            cancellationToken
        );

        var completedOrSkippedExerciseIds = exerciseHistories
            .Select(eh => eh.ExerciseId)
            .ToHashSet();

        var incompleteExercises = allExerciseIds
            .Where(exerciseId => !completedOrSkippedExerciseIds.Contains(exerciseId))
            .ToList();

        if (incompleteExercises.Any())
        {
            return Result.Failure(OngoingTrainingErrors.NotAllExercisesCompleted(request.Id));
        }

        ongoingTraining.Finish(finishedOnUtc: dateTimeProvider.UtcNow);

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
