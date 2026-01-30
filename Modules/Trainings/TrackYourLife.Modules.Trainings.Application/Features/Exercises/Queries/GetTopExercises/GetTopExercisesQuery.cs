using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;

public sealed record GetTopExercisesQuery(
    int Page,
    int PageSize,
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<PagedList<TopExerciseDto>>;
