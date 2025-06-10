using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public class DeleteOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingRepository
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

        ongoingTrainingRepository.Remove(ongoingTraining);

        return Result.Success();
    }
}
