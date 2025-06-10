using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public sealed record AdjustExerciseSetsCommand(
    OngoingTrainingId OngoingTrainingId,
    ExerciseId ExerciseId,
    List<ExerciseSetChange> ExerciseSetChanges
) : ICommand;
