using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes;

public class RecipeMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Mapping from entities to dtos
        config
            .NewConfig<IngredientReadModel, IngredientDto>()
            .Map(dest => dest.Food, src => src.Food.Adapt<FoodDto>(config))
            .Map(dest => dest.ServingSize, src => src.ServingSize.Adapt<ServingSizeDto>(config));

        config
            .NewConfig<RecipeReadModel, RecipeDto>()
            .Map(
                dest => dest.Ingredients,
                src => src.Ingredients.Select(i => i.Adapt<IngredientDto>(config))
            );

        // Mapping from requests to commands
        config.NewConfig<AddIngredientRequest, AddIngredientCommand>();
    }
}
