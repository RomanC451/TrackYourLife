using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Domain.Goals;

public static class GoalErrors
{
    public static readonly Func<GoalId, Error> NotOwned = id => Error.NotOwned(id, nameof(Goal));
    public static readonly Func<GoalId, Error> NotFound = id => Error.NotFound(id, nameof(Goal));

    public static readonly Func<GoalType, DateOnly, Error> AlreadyExists = (type, startDate) =>
        new Error(
            "UserGoal.AlreadyExists",
            $"The user '{type}' with the starting from {startDate}  already exists."
        );

    public static readonly Func<GoalType, Error> NotExisting = (type) =>
        new Error("UserGoal.NotExisting", $"The user doesn't have a {type} defined.", 404);

    public static readonly Func<GoalType, Error> Overlapping = (type) =>
        new Error(
            "UserGoal.Overlapping",
            $"The user already has a goal of type '{type}' that overlaps with the new goal."
        );
}
