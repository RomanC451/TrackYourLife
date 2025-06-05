using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;

public sealed record DeleteExerciseCommand(ExerciseId ExerciseId, bool ForceDelete) : ICommand;
