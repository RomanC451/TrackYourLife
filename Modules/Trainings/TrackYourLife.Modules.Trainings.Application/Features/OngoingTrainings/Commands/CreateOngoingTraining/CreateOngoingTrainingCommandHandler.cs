using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;

public sealed class CreateOngoingTrainingCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ITrainingsRepository trainingsRepository,
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateOngoingTrainingCommand, OngoingTrainingId>
{
    public async Task<Result<OngoingTrainingId>> Handle(
        CreateOngoingTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var training = await trainingsRepository.GetByIdAsync(
            request.TrainingId,
            cancellationToken
        );

        if (training is null)
        {
            return Result.Failure<OngoingTrainingId>(
                TrainingsErrors.NotFoundById(request.TrainingId)
            );
        }

        if (training.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<OngoingTrainingId>(TrainingsErrors.NotOwned(request.TrainingId));
        }

        var alreadyStartedTraining = await ongoingTrainingsRepository.GetUnfinishedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (alreadyStartedTraining is not null)
        {
            if (alreadyStartedTraining.Training.Id != request.TrainingId)
            {
                return Result.Failure<OngoingTrainingId>(
                    OngoingTrainingErrors.AnotherTrainingAlreadyStarted(alreadyStartedTraining.Id)
                );
            }

            return Result.Failure<OngoingTrainingId>(
                OngoingTrainingErrors.AlreadyStarted(alreadyStartedTraining.Id)
            );
        }

        var ongoingTraining = OngoingTraining.Create(
            id: OngoingTrainingId.NewId(),
            userId: userIdentifierProvider.UserId,
            training: training,
            startedOnUtc: dateTimeProvider.UtcNow
        );

        if (ongoingTraining.IsFailure)
        {
            return Result.Failure<OngoingTrainingId>(ongoingTraining.Error);
        }

        await ongoingTrainingsRepository.AddAsync(ongoingTraining.Value, cancellationToken);

        return Result.Success(ongoingTraining.Value.Id);
    }
}
