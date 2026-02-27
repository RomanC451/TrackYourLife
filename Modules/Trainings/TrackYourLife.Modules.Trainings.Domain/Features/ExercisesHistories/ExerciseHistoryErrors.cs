using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public static class ExerciseHistoryErrors
{
    public static readonly Func<ExerciseHistoryId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(ExerciseHistory));

    public static readonly Func<ExerciseHistoryId, Error> NotOwned = id =>
        Error.NotOwned(id, nameof(ExerciseHistory));
}
