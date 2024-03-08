using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Quartz;
using Scrutor;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Infrastructure.Options;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;
using TrackYourLifeDotnet.Infrastructure.Services;
using TrackYourLifeDotnet.Infrastructure.BackgroundJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using TrackYourLifeDotnet.Infrastructure.Configurations;
using TrackYourLifeDotnet.Infrastructure.Services.FoodApiService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddValidatorsFromAssembly(
            TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly
        );

        services.AddOptionsWithFluentValidation<JwtOptions>(JwtOptions.ConfigurationSection);
        services.AddOptionsWithFluentValidation<EmailOptions>(EmailOptions.ConfigurationSection);
        services.AddOptionsWithFluentValidation<FoodApiOptions>(
            FoodApiOptions.ConfigurationSection
        );
        services.AddOptionsWithFluentValidation<UnitConversionOptions>(
            UnitConversionOptions.ConfigurationSection
        );

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddHttpClient<IFoodApiService, FoodApiService>().ConfigureFoodApiHttpClient();

        services.Scan(
            selector =>
                selector
                    .FromAssemblies(TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly)
                    .AddClasses(
                        classes =>
                            classes.Where(
                                type =>
                                    (
                                        type.Name.EndsWith("Service")
                                        || (
                                            type.Namespace != null
                                            && type.Namespace.EndsWith("Authentication")
                                        )
                                    )
                            )
                    )
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // services.AddQuartzHostedService();
        // services.AddQuartz(configure =>
        // {
        //     var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

        //     configure
        //         .AddJob<ProcessOutboxMessagesJob>(jobKey)
        //         .AddTrigger(
        //             trigger =>
        //                 trigger
        //                     .ForJob(jobKey)
        //                     .WithSimpleSchedule(
        //                         schedule => schedule.WithIntervalInSeconds(10).RepeatForever()
        //                     )
        //         );
        // });

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        return services;
    }
}
