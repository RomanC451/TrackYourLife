using Mapster;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries;

internal sealed class RecipeDiaryMappingsConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //Read models to DTOs
        config
            .NewConfig<RecipeDiaryReadModel, RecipeDiaryDto>()
            .Map(dest => dest.Recipe, src => src.Recipe.Adapt<RecipeDto>(config));

        //Requests to Commands
        config.NewConfig<AddRecipeDiaryRequest, AddRecipeDiaryCommand>();
        config.NewConfig<UpdateRecipeDiaryRequest, UpdateRecipeDiaryCommand>();
    }
}
