using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;

public sealed record UpdateNutritionGoalsCommand(
    int Calories,
    int Protein,
    int Carbohydrates,
    int Fats,
    bool Force
) : ICommand;
