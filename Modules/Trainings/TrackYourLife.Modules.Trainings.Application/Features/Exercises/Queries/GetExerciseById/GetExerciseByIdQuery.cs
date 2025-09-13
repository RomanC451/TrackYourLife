using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;

public sealed record GetExerciseByIdQuery(ExerciseId Id) : IQuery<ExerciseReadModel>;
