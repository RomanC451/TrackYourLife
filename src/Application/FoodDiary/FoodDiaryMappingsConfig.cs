using Mapster;
using TrackYourLifeDotnet.Application.FoodDiary.Commands.AddFoodDieryEntry;
using TrackYourLifeDotnet.Application.FoodDiary.Commands.RemoveFoodDiaryEntry;
using TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Application.FoodDiary;

public sealed class FoodDiaryMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodDiaryEntry, FoodDiaryEntryDto>();

        config
            .NewConfig<GetFoodDiaryRequest, GetFoodDiaryQuery>()
            .Map(dest => dest.Date, src => src.Date);

        config
            .NewConfig<GetFoodDiaryResult, GetFoodDiaryResponse>()
            .Map(dest => dest.Breakfast, src => src.Breakfast);

        config
            .NewConfig<AddFoodDiaryEntryRequest, AddFoodDiaryEntryCommand>()
            .Map(dest => dest.FoodId, src => new FoodId(src.FoodId!.Value))
            .Map(dest => dest.MealType, src => src.MealType)
            .Map(dest => dest.ServingSizeId, src => new ServingSizeId(src.ServingSizeId!.Value))
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.Date, src => src.Date);

        config
            .NewConfig<AddFoodDiaryEntryResult, AddFoodDiaryEntryResponse>()
            .Map(dest => dest.FoodDiaryEntryId, src => src.FoodDiaryEntryId.Value);

        config
            .NewConfig<RemoveFoodDiaryEntryRequest, RemoveFoodDiaryEntryCommand>()
            .Map(
                dest => dest.FoodDiaryEntryId,
                src => new FoodDiaryEntryId(src.FoodDiaryEntryId!.Value)
            );
    }
}
