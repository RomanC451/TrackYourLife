using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public interface IOngoingTrainingsRepository
{
    Task<OngoingTraining?> GetByIdAsync(
        OngoingTrainingId ongoingTrainingId,
        CancellationToken cancellationToken = default
    );

    Task<OngoingTraining?> GetUnfinishedByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(OngoingTraining ongoingTraining, CancellationToken cancellationToken = default);

    void Update(OngoingTraining ongoingTraining);
}
