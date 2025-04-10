using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Goals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;

public sealed record GetNutritionGoalsQuery(DateOnly Date) : IQuery<List<GoalReadModel>>;
