using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public sealed record GetOngoingTrainingByUserIdQuery() : IQuery<OngoingTrainingReadModel>;
