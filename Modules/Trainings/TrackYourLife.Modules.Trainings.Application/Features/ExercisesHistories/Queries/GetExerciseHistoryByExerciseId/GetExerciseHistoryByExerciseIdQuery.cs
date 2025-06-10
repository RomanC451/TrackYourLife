using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;

public sealed record GetExerciseHistoryByExerciseIdQuery(ExerciseId ExerciseId)
    : IQuery<IEnumerable<ExerciseHistoryReadModel>>;
