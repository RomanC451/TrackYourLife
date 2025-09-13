using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

public static class OngoingTrainingErrors
{
    public static readonly Func<OngoingTrainingId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(OngoingTraining));

    public static readonly Func<OngoingTrainingId, Error> NotOwned = (id) =>
        Error.NotOwned(id, nameof(OngoingTraining));

    public static readonly Func<OngoingTrainingId, Error> AlreadyFinished = (id) =>
        new(
            "OngoingTraining.AlreadyFinished",
            $"The ongoing training with id {id} is already finished."
        );

    public static readonly Func<OngoingTrainingId, Error> AlreadyStarted = (id) =>
        new(
            "OngoingTraining.AlreadyStarted",
            $"The ongoing training with id {id} is already started."
        );

    public static readonly Func<OngoingTrainingId, Error> AnotherTrainingAlreadyStarted = (id) =>
        new(
            "OngoingTraining.AnotherTrainingAlreadyStarted",
            $"There is another ongoing training with id {id} for already started."
        );

    public static readonly Error NotFound = new(
        "OngoingTraining.NotFound",
        "No ongoing training found.",
        404
    );

    public static readonly Func<OngoingTrainingId, Error> NoNext = (id) =>
        new(
            "OngoingTraining.NoNext",
            $"The ongoing training with id {id} has no next set or exercise."
        );

    public static readonly Func<OngoingTrainingId, Error> NoPrevious = (id) =>
        new(
            "OngoingTraining.NoPrevious",
            $"The ongoing training with id {id} has no previous set or exercise."
        );
}
