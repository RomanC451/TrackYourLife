using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries;

/// <summary>
/// Represents the configuration for mapping between different types related to food diaries.
/// </summary>
public sealed class FoodDiaryMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Read models to DTOs
        config
            .NewConfig<FoodDiaryReadModel, FoodDiaryDto>()
            .Map(dest => dest.Food, src => src.Food.Adapt<FoodDto>(config))
            .Map(dest => dest.ServingSize, src => src.ServingSize.Adapt<ServingSizeDto>(config));

        //Requests to Commands
        config.NewConfig<AddFoodDiaryRequest, AddFoodDiaryCommand>();
        config.NewConfig<UpdateFoodDiaryRequest, UpdateFoodDiaryCommand>();
    }
}
