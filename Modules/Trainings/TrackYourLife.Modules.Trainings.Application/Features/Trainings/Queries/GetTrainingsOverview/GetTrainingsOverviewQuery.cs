using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;

public sealed record GetTrainingsOverviewQuery(DateOnly? StartDate, DateOnly? EndDate)
    : IQuery<TrainingsOverviewDto>;
