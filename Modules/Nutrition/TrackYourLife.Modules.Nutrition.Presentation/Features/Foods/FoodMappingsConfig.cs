using Mapster;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods;

/// <summary>
/// Represents a configuration class for mapping between different types related to foods.
/// </summary>
public sealed class FoodMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Read models to DTOs
        config.NewConfig<ServingSize, ServingSizeDto>();

        config
            .NewConfig<FoodReadModel, FoodDto>()
            .Map(
                dest => dest.ServingSizes,
                src =>
                    src.FoodServingSizes.ToDictionary(
                        fss => fss.Index,
                        fss => fss.ServingSize.Adapt<ServingSizeDto>(config)
                    )
            );
    }
}
