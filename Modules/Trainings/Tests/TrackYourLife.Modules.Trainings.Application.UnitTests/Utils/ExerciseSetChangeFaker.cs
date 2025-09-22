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
        return new ExerciseSetChange
        {
            SetId = setId ?? Guid.NewGuid(),
            WeightChange = weightChange ?? f.Random.Float(-10, 10),
            RepsChange = repsChange ?? f.Random.Int(-5, 5)
        };
    }
}
