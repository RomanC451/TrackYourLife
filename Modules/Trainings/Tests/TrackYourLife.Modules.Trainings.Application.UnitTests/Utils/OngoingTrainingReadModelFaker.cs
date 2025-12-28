using Bogus;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class OngoingTrainingReadModelFaker
{
    private static readonly Faker f = new();

    public static OngoingTrainingReadModel Generate(
        OngoingTrainingId? id = null,
        UserId? userId = null,
        int? exerciseIndex = null,
        int? setIndex = null,
        DateTime? startedOnUtc = null,
        DateTime? finishedOnUtc = null,
        TrainingReadModel? training = null
    )
    {
        var ongoingTraining = new OngoingTrainingReadModel(
            id ?? OngoingTrainingId.NewId(),
            userId ?? UserId.NewId(),
            exerciseIndex ?? f.Random.Int(0, 5),
            setIndex ?? f.Random.Int(0, 3),
            startedOnUtc ?? f.Date.Recent(),
            finishedOnUtc
        );

        if (training != null)
        {
            ongoingTraining.Training = training;
        }

        return ongoingTraining;
    }
}
