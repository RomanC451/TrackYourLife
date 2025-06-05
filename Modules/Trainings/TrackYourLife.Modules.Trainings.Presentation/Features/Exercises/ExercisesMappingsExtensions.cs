using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;

internal static class ExercisesMappingsExtensions
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
}
