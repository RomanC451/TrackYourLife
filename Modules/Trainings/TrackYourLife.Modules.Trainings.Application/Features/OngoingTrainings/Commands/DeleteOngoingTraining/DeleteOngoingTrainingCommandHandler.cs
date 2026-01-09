using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public class DeleteOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingRepository,
    IExercisesHistoriesRepository exercisesHistoriesRepository
) : ICommandHandler<DeleteOngoingTrainingCommand>
{
    public async Task<Result> Handle(
        DeleteOngoingTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingRepository.GetByTrainingIdAndNotFinishedAsync(
            request.TrainingId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure(OngoingTrainingErrors.NotFound);
        }

        // Get all exercise histories for this ongoing training
        var exerciseHistories = await exercisesHistoriesRepository.GetByOngoingTrainingIdAsync(
            ongoingTraining.Id,
            cancellationToken
        );

        // Remove all exercise histories
        foreach (var exerciseHistory in exerciseHistories)
        {
            exercisesHistoriesRepository.Remove(exerciseHistory);
        }

        // Remove the ongoing training
        ongoingTrainingRepository.Remove(ongoingTraining);

        return Result.Success();
    }
}
