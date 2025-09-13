using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings.Events;

public sealed record OngoingTrainingFinishedDomainEvent(OngoingTrainingId OngoingTrainingId)
    : IDirectDomainEvent;
