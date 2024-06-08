using Mapster;
using TrackYourLife.Common.Application.FoodDiaries.Commands.AddFoodDiaryEntry;
using TrackYourLife.Common.Application.FoodDiaries.Commands.UpdateFoodDiaryEntry;
using TrackYourLife.Common.Application.FoodDiaries.Queries.GetFoodDiaryByDate;
using TrackYourLife.Common.Application.FoodDiaries.Queries.GetTotalCaloriesByPeriod;
using TrackYourLife.Common.Contracts.FoodDiaries;
using TrackYourLife.Common.Domain.FoodDiaries;

namespace TrackYourLife.Common.Application.FoodDiaries;

public sealed class FoodDiaryMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodDiaryEntry, FoodDiaryEntryResponse>()
            .Map(dest => dest.EntryDate, src => src.Date);

        config
            .NewConfig<GetFoodDiaryByDateRequest, GetFoodDiaryByDateQuery>();

        config
            .NewConfig<AddFoodDiaryEntryRequest, AddFoodDiaryEntryCommand>();

        config
            .NewConfig<UpdateFoodDiaryEntryRequest, UpdateFoodDiaryEntryCommand>();

        config
            .NewConfig<GetTotalCaloriesByPeriodRequest, GetTotalCaloriesByPeriodQuery>();
    }
}
