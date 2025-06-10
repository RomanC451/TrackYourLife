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
            PictureUrl: exercise.PictureUrl,
            VideoUrl: exercise.VideoUrl,
            Description: exercise.Description,
            Equipment: exercise.Equipment,
            ExerciseSets: exercise.ExerciseSets,
            CreatedOnUtc: exercise.CreatedOnUtc,
            ModifiedOnUtc: exercise.ModifiedOnUtc
        );
    }

    public static ExerciseSetDto ToDto(this ExerciseSet exerciseSet)
    {
        return new ExerciseSetDto(
            Id: exerciseSet.Id,
            Name: exerciseSet.Name,
            Reps: exerciseSet.Reps,
            Weight: exerciseSet.Weight,
            OrderIndex: exerciseSet.OrderIndex
        );
    }

    public static ExerciseSet ToEntity(this ExerciseSetDto exerciseSet)
    {
        return new ExerciseSet(
            exerciseSet.Id ?? Guid.NewGuid(),
            exerciseSet.Name,
            exerciseSet.Reps,
            exerciseSet.Weight,
            exerciseSet.OrderIndex
        );
    }
}
