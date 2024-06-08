using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Quartz;
using Scrutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TrackYourLife.Common.Infrastructure.Configurations;
using TrackYourLife.Common.Infrastructure.Health;
using TrackYourLife.Common.Infrastructure.Authentication;
using TrackYourLife.Common.Infrastructure.BackgroundJobs;
using TrackYourLife.Common.Infrastructure.Options;
using TrackYourLife.Common.Infrastructure.Services;
using TrackYourLife.Common.Infrastructure.Swagger;
using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Services;

namespace TrackYourLife.Common.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        services.AddOptionsWithFluentValidation<FoodApiOptions>(
            FoodApiOptions.ConfigurationSection
        );
        services.AddOptionsWithFluentValidation<SupaBaseOptions>(
            SupaBaseOptions.ConfigurationSection
        );


        services.AddScoped<IUserIdentifierProvider, UserIdentifierProvider>();

        services.AddHttpClient<IFoodApiService, FoodApiService>().ConfigureFoodApiHttpClient();

        services.Scan(
            selector =>
                selector
                    .FromAssemblies(AssemblyReference.Assembly)
                    .AddClasses(
                        classes =>
                            classes.Where(
                                type =>
                                    type.Name.EndsWith("Service")
                                    || type.Namespace != null
                                        && type.Namespace.EndsWith("Authentication")
                            )
                    )
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        services.AddEndpointsApiExplorer();
        services.AddCustomSwaggerGen();

        services.AddQuartzHostedService();
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(
                    trigger =>
                        trigger
                            .ForJob(jobKey)
                            .WithSimpleSchedule(
                                schedule => schedule.WithIntervalInSeconds(10).RepeatForever()
                            )
                );
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddOptions();
        services.AddMemoryCache();

        services
            .AddHealthChecks()
            .AddCheck<FoodApiServiceHealthCheck>(
                "FoodApiServiceHealthCheck",
                HealthStatus.Unhealthy
            )
            .AddNpgSql(configuration.GetConnectionString("Database")!);

        services.AddScoped<Supabase.Client>(
            (provider) =>
            {
                var supaBaseOptions = provider
                    .GetRequiredService<IOptions<SupaBaseOptions>>()
                    .Value;
                ;

                return new Supabase.Client(supaBaseOptions.Url, supaBaseOptions.Key);
            }
        );

        return services;
    }
}
