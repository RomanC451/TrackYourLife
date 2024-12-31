using Mapster;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries;

public class NutritionDiaryMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Mapping from read model to dtos
        config
            .NewConfig<FoodDiaryReadModel, NutritionDiaryDto>()
            .Map(dest => dest.Name, src => $"{src.Food.Name} ({src.Food.BrandName})")
            .Map(dest => dest.NutritionMultiplier, src => src.ServingSize.NutritionMultiplier)
            .Map(dest => dest.ServingSize, src => src.ServingSize.Adapt<ServingSizeDto>(config))
            .Map(
                dest => dest.NutritionalContents,
                src =>
                    src.Food.NutritionalContents.MultiplyNutritionalValues(
                        src.ServingSize.NutritionMultiplier
                    )
            )
            .Map(dest => dest.DiaryType, _ => DiaryType.FoodDiary);

        config
            .NewConfig<RecipeDiaryReadModel, NutritionDiaryDto>()
            .Map(
                dest => dest.Name,
                src => src.Recipe.IsOld ? $"{src.Recipe.Name} (old)" : src.Recipe.Name
            )
            .Map(dest => dest.NutritionMultiplier, _ => 1)
            .Map(dest => dest.ServingSize, _ => (ServingSizeDto)null!)
            .Map(dest => dest.NutritionalContents, src => src.Recipe.NutritionalContents)
            .Map(dest => dest.DiaryType, _ => DiaryType.RecipeDiary)
            .Map(dest => dest.DiaryType, _ => DiaryType.RecipeDiary);
    }
}
