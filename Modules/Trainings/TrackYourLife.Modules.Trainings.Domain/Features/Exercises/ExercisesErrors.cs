using TrackYourLife.Modules.Trainings.Domain.Features.Sets;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public static class ExercisesErrors
{
    public static readonly Func<ExerciseId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(Exercise));

    public static readonly Func<ExerciseId, Error> NotOwned = (id) =>
        Error.NotOwned(id, nameof(Exercise));

    public static readonly Func<string, Error> SetNotFound = name =>
        new(
            $"{nameof(ExerciseSet)}.NotFound",
            $"{nameof(ExerciseSet)} with name {name} was not found.",
            404
        );

    public static readonly Func<ExerciseId, Error> UsedInTrainings = id =>
        new(
            $"{nameof(Exercise)}.UsedInTrainings",
            $"Exercise with id {id} is used in trainings. Use the force delete option to delete it."
        );
}
