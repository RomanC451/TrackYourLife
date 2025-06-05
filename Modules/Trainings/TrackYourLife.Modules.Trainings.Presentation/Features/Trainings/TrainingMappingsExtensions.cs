using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;

/// <summary>
/// Represents the extension class for mapping between different types related to trainings.
/// </summary>

internal static class TrainingMappingsExtensions
{
    public static TrainingDto ToDto(this TrainingReadModel training)
    {
        return new TrainingDto(
            Id: training.Id,
            Name: training.Name,
            Duration: training.Duration,
            Description: training.Description,
            Exercises: training
                .TrainingExercises.OrderBy(e => e.OrderIndex)
                .Select(e => e.Exercise.ToDto())
                .ToList(),
            CreatedOnUtc: training.CreatedOnUtc,
            ModifiedOnUtc: training.ModifiedOnUtc
        );
    }
}
