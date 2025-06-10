using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.AdjustExerciseSets;

public sealed record AdjustExerciseSetsCommand(
    ExerciseId ExerciseId,
    List<ExerciseSetChange> ExerciseSetChanges
) : ICommand;
