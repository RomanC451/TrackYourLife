using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;

public sealed record GetTrainingTemplatesUsageQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null
) : IQuery<IEnumerable<TrainingTemplateUsageDto>>;
