using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipeName;

public sealed record UpdateRecipeNameCommand(RecipeId RecipeId, string Name) : ICommand;
