using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Goals;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.RemoveGoal;

public sealed record RemoveGoalCommand(GoalId Id) : ICommand;
