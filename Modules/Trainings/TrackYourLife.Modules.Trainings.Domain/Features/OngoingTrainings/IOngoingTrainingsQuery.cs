using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public interface IOngoingTrainingsQuery
{
    Task<OngoingTrainingReadModel?> GetByIdAsync(
        OngoingTrainingId id,
        CancellationToken cancellationToken = default
    );

    Task<OngoingTrainingReadModel?> GetUnfinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> IsTrainingOngoingAsync(
        TrainingId trainingId,
        CancellationToken cancellationToken = default
    );
}
