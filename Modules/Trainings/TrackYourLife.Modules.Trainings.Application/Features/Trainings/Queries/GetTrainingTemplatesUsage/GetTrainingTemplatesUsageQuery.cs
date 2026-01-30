using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;

public sealed record GetTrainingTemplatesUsageQuery(
    DateTime? StartDate = null,
    DateTime? EndDate = null
) : IQuery<IEnumerable<TrainingTemplateUsageDto>>;
