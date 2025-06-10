using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;

public class DeleteTrainingCommandHandler(
    ITrainingsRepository trainingsRepository,
    IOngoingTrainingsQuery ongoingTrainingsQuery,
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

        var isTrainingOngoing = await ongoingTrainingsQuery.IsTrainingOngoingAsync(
            request.TrainingId,
            cancellationToken
        );

        if (isTrainingOngoing && !request.Force)
        {
            return Result.Failure(TrainingsErrors.OngoingTraining(request.TrainingId));
        }

        trainingsRepository.Remove(training);

        return Result.Success();
    }
}
