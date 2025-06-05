using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public interface ITrainingsQuery
{
    Task<TrainingReadModel?> GetByIdAsync(
        TrainingId id,
        CancellationToken cancellationToken = default
    );

    Task<IEnumerable<TrainingReadModel>> GetTrainingsByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> ExistsByUserIdAndNameAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    );
}
