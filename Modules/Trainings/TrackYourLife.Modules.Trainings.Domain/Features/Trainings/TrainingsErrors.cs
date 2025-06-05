using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

public static class TrainingsErrors
{
    public static readonly Func<TrainingId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(Training));

    public static readonly Func<TrainingId, Error> NotOwned = (id) =>
        Error.NotOwned(id, nameof(Training));
}
