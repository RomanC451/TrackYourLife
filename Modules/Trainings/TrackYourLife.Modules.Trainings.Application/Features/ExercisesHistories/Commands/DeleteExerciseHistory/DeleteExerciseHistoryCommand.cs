using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Commands.DeleteExerciseHistory;

public sealed record DeleteExerciseHistoryCommand(ExerciseHistoryId ExerciseHistoryId) : ICommand;
