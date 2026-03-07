using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.MuscleGroups.Queries.GetMuscleGroups;

public sealed record GetMuscleGroupsQuery : IQuery<IReadOnlyList<MuscleGroupDto>>;
