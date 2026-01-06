using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Nutrition.Application.Core.Behaviors;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Behaviors;

namespace TrackYourLife.Modules.Nutrition.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddNutritionApplicationServices(
        this IServiceCollection services
    )
    {
        // Add validators from the assembly
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(AssemblyReference.Assembly);

            cfg.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(NutritionUnitOfWorkBehavior<,>));
        });

        //Add domain services
        services.AddScoped<IRecipeService, RecipeService>();

        //Add application services
        services.AddScoped<IFoodHistoryService, FoodHistoryService>();

        return services;
    }
}
