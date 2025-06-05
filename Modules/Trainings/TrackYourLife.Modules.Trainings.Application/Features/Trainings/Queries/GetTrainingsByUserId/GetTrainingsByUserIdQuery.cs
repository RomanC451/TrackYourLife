using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;

public sealed record GetTrainingsByUserIdQuery : IQuery<IEnumerable<TrainingReadModel>>;
