using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;

internal static class ExerciseMappingsExtensions
{
    public static ExerciseDto ToDto(this ExerciseReadModel exercise)
    {
        return new ExerciseDto(
            Id: exercise.Id,
            Name: exercise.Name,
            MuscleGroups: exercise.MuscleGroups,
            Difficulty: exercise.Difficulty,
            PictureUrl: exercise.PictureUrl,
            VideoUrl: exercise.VideoUrl,
            Description: exercise.Description,
            Equipment: exercise.Equipment,
            ExerciseSets: exercise.ExerciseSets.ToList(),
            CreatedOnUtc: exercise.CreatedOnUtc,
            ModifiedOnUtc: exercise.ModifiedOnUtc
        );
    }

    public static ExerciseSet EnsureHaveId(this ExerciseSet exerciseSet)
    {
        exerciseSet.Id = Guid.NewGuid();

        return exerciseSet;
    }
}
