using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;

public sealed record GetDifficultyDistributionQuery(DateOnly? StartDate, DateOnly? EndDate)
    : IQuery<IReadOnlyList<DifficultyDistributionDto>>;
