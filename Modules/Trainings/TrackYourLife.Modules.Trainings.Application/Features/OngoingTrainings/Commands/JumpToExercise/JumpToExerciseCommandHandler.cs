using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;

public sealed class JumpToExerciseCommandHandler(
    IOngoingTrainingsRepository ongoingTrainingsRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<JumpToExerciseCommand>
{
    public async Task<Result> Handle(
        JumpToExerciseCommand request,
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

        if (ongoingTraining.IsFinished)
        {
            return Result.Failure(OngoingTrainingErrors.AlreadyFinished(request.OngoingTrainingId));
        }

        var result = ongoingTraining.JumpToExercise(request.ExerciseIndex);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        ongoingTrainingsRepository.Update(ongoingTraining);

        return Result.Success();
    }
}
