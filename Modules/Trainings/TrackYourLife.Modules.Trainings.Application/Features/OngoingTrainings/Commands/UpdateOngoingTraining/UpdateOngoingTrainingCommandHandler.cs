using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.UpdateOngoingTraining;

public sealed class UpdateOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateOngoingTrainingCommand>
{
    public async Task<Result> Handle(
        UpdateOngoingTrainingCommand request,
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

        if (!ongoingTraining.IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.NotFinished(request.Id));
        }

        var finishedOnUtc = ongoingTraining.StartedOnUtc.AddMinutes(request.DurationMinutes);

        var updateResult = ongoingTraining.UpdateCompletionMetadata(
            finishedOnUtc: finishedOnUtc,
            caloriesBurned: request.CaloriesBurned
        );

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
