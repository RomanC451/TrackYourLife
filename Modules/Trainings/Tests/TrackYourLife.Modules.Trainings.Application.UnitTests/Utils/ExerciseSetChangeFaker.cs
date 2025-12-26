using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class ExerciseSetChangeFaker
{
    private static readonly Faker f = new();

    public static ExerciseSetChange Generate(
        Guid? setId = null,
        float? weightChange = null,
        int? repsChange = null
    )
    {
        return new WeightBasedExerciseSetChange(
            setId ?? Guid.NewGuid(),
            weightChange ?? f.Random.Float(0, 10),
            repsChange ?? f.Random.Int(0, 5)
        );
    }
}
