using Mapster;
using TrackYourLife.Common.Contracts.Foods;
using TrackYourLife.Common.Application.Foods.Queries.GetFoodList;
using TrackYourLife.Common.Domain.Foods;

namespace TrackYourLife.Common.Application.Foods;

public sealed class FoodMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<GetFoodListRequest, GetFoodListQuery>()
            .Map(dest => dest.SearchParam, src => src.SearchParam)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);
        ;

        config
            .NewConfig<Food, FoodResponse>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(
                dest => dest.ServingSizes,
                src =>
                    src.FoodServingSizes
                        .Select(
                            ss =>
                                new ServingSizeResponse(
                                    ss.ServingSize.Id,
                                    ss.ServingSize.NutritionMultiplier,
                                    ss.ServingSize.Value,
                                    ss.ServingSize.Unit,
                                    ss.Index
                                )
                        )
                        .ToList()
            );
    }
}
