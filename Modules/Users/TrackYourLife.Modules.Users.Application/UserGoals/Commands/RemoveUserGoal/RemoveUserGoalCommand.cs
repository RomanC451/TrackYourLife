using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Application.UserGoals.Commands.RemoveUserGoal;

public sealed record RemoveUserGoalCommand(UserGoalId Id) : ICommand;
