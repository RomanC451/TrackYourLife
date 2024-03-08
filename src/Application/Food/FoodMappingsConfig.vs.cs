using Mapster;
using TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;
using TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.Dtos;

namespace TrackYourLifeDotnet.Application.Foods;

public sealed class FoodMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<GetFoodListRequest, GetFoodListQuery>()
            .Map(dest => dest.SearchParam, src => src.SearchParam)
            .Map(dest => dest.Page, src => src.Page)
            .Map(dest => dest.PageSize, src => src.PageSize);

        config
            .NewConfig<GetFoodDiaryResult, GetFoodDiaryResponse>()
            .ConstructUsing(
                src => new GetFoodDiaryResponse(src.Breakfast, src.Lunch, src.Dinner, src.Snacks)
            );

        config
            .NewConfig<Food, FoodDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(
                dest => dest.ServingSizes,
                src =>
                    src.FoodServingSizes
                        .Select(
                            ss =>
                                new ServingSizeDto(
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
