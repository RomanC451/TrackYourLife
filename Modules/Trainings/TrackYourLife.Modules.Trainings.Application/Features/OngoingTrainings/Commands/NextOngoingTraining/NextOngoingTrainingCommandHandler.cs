using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;

public class NextOngoingTrainingCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<NextOngoingTrainingCommand>
{
    public async Task<Result> Handle(
        NextOngoingTrainingCommand request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsRepository.GetByIdAsync(
            request.OngoingTrainingId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure(OngoingTrainingErrors.NotFoundById(request.OngoingTrainingId));
        }

        if (ongoingTraining.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(OngoingTrainingErrors.NotOwned(request.OngoingTrainingId));
        }

        var result = ongoingTraining.Next();

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
