
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;


public static class UserGoalErrors
{
    public static readonly Func<UserGoalId, Error> NotOwned = userGoalId =>
        new Error(
            "UserGoal.NotOwned",
            $"The user goal with the Id '{userGoalId.Value}' does not belong to the current user."
        );
    public static readonly Func<UserGoalId, Error> NotFound = userGoalId =>
        new Error(
            "UserGoal.NotFound",
            $"The user goal with the Id '{userGoalId.Value}' was not found."
        );

    public static readonly Func<UserGoalType, DateOnly, Error> AlreadyExists = (
        type,
        startDate
    ) =>
        new Error(
            "UserGoal.AlreadyExists",
            $"The user '{type}' with the starting from {startDate}  already exists."
        );

    public static readonly Func<UserGoalType, Error> NotExisting = (type) =>
        new Error("UserGoal.NotExisting", $"The user doesn't have a {type} defined.");

    public static readonly Func<UserGoalType, Error> Overlapping = (type) =>
        new Error(
            "UserGoal.Overlapping",
            $"The user already has a goal of type '{type}' that overlaps with the new goal."
        );
}
