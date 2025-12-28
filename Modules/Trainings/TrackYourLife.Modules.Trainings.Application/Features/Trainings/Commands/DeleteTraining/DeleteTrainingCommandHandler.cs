using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;

public class DeleteTrainingCommandHandler(
    ITrainingsRepository trainingsRepository,
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteTrainingCommand>
{
    public async Task<Result> Handle(
        DeleteTrainingCommand request,
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

        var ongoingTraining = await ongoingTrainingsRepository.GetByTrainingIdAndNotFinishedAsync(
            request.TrainingId,
            cancellationToken
        );

        if (ongoingTraining is not null && !request.Force)
        {
            return Result.Failure(TrainingsErrors.OngoingTraining(request.TrainingId));
        }
        if (ongoingTraining is not null)
        {
            ongoingTrainingsRepository.Remove(ongoingTraining);
        }
        trainingsRepository.Remove(training);

        return Result.Success();
    }
}
