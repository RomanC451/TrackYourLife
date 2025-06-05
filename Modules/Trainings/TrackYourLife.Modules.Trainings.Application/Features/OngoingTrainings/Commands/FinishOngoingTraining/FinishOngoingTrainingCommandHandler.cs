using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public sealed class FinishOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
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

        ongoingTraining.Finish(dateTimeProvider.UtcNow);

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
