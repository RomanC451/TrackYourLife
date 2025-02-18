using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;

public sealed record CalculateNutritionGoalsCommand(
    int Age,
    int Weight,
    int Height,
    Gender Gender,
    ActivityLevel ActivityLevel,
    FitnessGoal FitnessGoal,
    bool Force
) : ICommand;
