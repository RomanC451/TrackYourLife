using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public sealed record GetOngoingTrainingByIdQuery(OngoingTrainingId Id)
    : IQuery<(
        OngoingTrainingReadModel OngoingTraining,
        IEnumerable<ExerciseHistoryReadModel> ExerciseHistories
    )>;
