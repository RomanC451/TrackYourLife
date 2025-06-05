using FluentValidation;
using MassTransit.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Events;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Nutrition.Infrastructure.Configurations;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Health;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.ShutdownJobs;
using TrackYourLife.Modules.Nutrition.Infrastructure.StartupJobs;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Nutrition.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddNutritionInfrastructureServices(
        this IServiceCollection services
    )
    {
        //Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        //Add options
        services.AddOptionsWithFluentValidation<FoodApiOptions>(
            FoodApiOptions.ConfigurationSection
        );

        //Add Http clients
        services.AddHttpClient<IFoodApiService, FoodApiService>().ConfigureFoodApiHttpClient();

        //Add health checks
        services
            .AddHealthChecks()
            .AddCheck<FoodApiServiceHealthCheck>("FoodApiService", HealthStatus.Unhealthy);

        //Add db contexts
        services.AddDbContext<NutritionWriteDbContext>();
        services.AddDbContext<NutritionReadDbContext>();

        //Add repositories
        services.RegisterRepositories(AssemblyReference.Assembly);

        services.Decorate<IFoodQuery, CachedFoodQuery>();

        services.AddScoped<INutritionUnitOfWork, NutritionUnitOfWork>();

        //Add consumers
        services.RegisterConsumer<NutritionGoalUpdatedIntegrationEventConsummer>();

        //Add Background jobs
        services.AddQuartz(configure =>
        {
            var keepFoodApiAuthenticatedJobJobKey = new JobKey(nameof(KeepFoodApiAuthenticatedJob));

            configure
                .AddJob<KeepFoodApiAuthenticatedJob>(keepFoodApiAuthenticatedJobJobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(keepFoodApiAuthenticatedJobJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInMinutes(10).RepeatForever()
                        )
                );

            var initializeFoodApiCookieContainerJobKey = new JobKey(
                nameof(InitializeFoodApiCookieContainerJob)
            );

            configure
                .AddJob<InitializeFoodApiCookieContainerJob>(initializeFoodApiCookieContainerJobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(initializeFoodApiCookieContainerJobKey)
                        .StartNow()
                        .WithSimpleSchedule(x => x.WithRepeatCount(0))
                );

            configure.AddJob<SaveFoodApiCookiesJob>(jobKey =>
                jobKey.WithIdentity(nameof(SaveFoodApiCookiesJob)).StoreDurably()
            );

            var outboxJobKey = new JobKey($"{nameof(ProcessNutritionOutboxMessagesJob)}");

            configure
                .AddJob<ProcessNutritionOutboxMessagesJob>(outboxJobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(outboxJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10).RepeatForever()
                        )
                );
        });

        //Add domain services
        services.AddScoped<IRecipeService, RecipeService>();

        //Add external services
        services.AddSingleton(new FoodApiCookieContainer());
        services.AddScoped<IFoodApiCookiesManager, FoodApiCookiesManager>();
        services.AddScoped<IFoodHistoryService, FoodHistoryService>();
        services.AddScoped<IRecipeService, RecipeService>();
        services.AddSingleton<IFoodApiAuthDataStore, FoodApiAuthDataStore>();

        return services;
    }
}
