using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class OngoingTrainingFaker
{
    private static readonly Faker f = new();

    public static OngoingTraining Generate(
        OngoingTrainingId? id = null,
        UserId? userId = null,
        int? exerciseIndex = null,
        int? setIndex = null,
        DateTime? startedOnUtc = null,
        DateTime? finishedOnUtc = null,
        Training? training = null
    )
    {
        var trainingEntity = training ?? TrainingFaker.Generate();

        // If specific indices are provided, use reflection to call the private constructor
        if (exerciseIndex.HasValue || setIndex.HasValue)
        {
            var constructor = typeof(OngoingTraining).GetConstructor(
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new[]
                {
                    typeof(OngoingTrainingId),
                    typeof(UserId),
                    typeof(Training),
                    typeof(int),
                    typeof(int),
                    typeof(DateTime),
                },
                null
            );

            if (constructor != null)
            {
                var ongoingTraining = (OngoingTraining)
                    constructor.Invoke(
                        new object[]
                        {
                            id ?? OngoingTrainingId.NewId(),
                            userId ?? UserId.NewId(),
                            trainingEntity,
                            exerciseIndex ?? 0,
                            setIndex ?? 0,
                            startedOnUtc ?? f.Date.Recent(),
                        }
                    );
                return ongoingTraining;
            }
        }

        // Fallback to the normal Create method
        var result = OngoingTraining.Create(
            id ?? OngoingTrainingId.NewId(),
            userId ?? UserId.NewId(),
            trainingEntity,
            startedOnUtc ?? f.Date.Recent()
        );

        return result.Value;
    }
}
