using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Application.Features.Goals.Consumers;
using TrackYourLife.Modules.Users.Infrastructure.Authentication;
using TrackYourLife.Modules.Users.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Extensions;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.OptionsSetup;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Contracts.Integration.Busses;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Users.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddUsersInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        //Add db contexts
        services.AddDbContext<UsersWriteDbContext>();
        services.AddDbContext<UsersReadDbContext>();

        //Add Background jobs
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey($"{nameof(ProcessOutboxMessagesJob)}-Users");

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10).RepeatForever()
                        )
                );
        });

        //Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        //Add options
        services.AddOptionsWithFluentValidation<RefreshTokenCookieOptions>(
            RefreshTokenCookieOptions.ConfigurationSection
        );
        services.AddOptionsWithFluentValidation<JwtOptions>(JwtOptions.ConfigurationSection);
        services.AddOptionsWithFluentValidation<EmailOptions>(EmailOptions.ConfigurationSection);
        services.AddOptionsWithFluentValidation<ClientAppOptions>(
            ClientAppOptions.ConfigurationSection
        );

        //Add MassTransit
        services.AddMassTransit<IUsersBus>(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<GetNutritionGoalsByUserIdConsumer>();

            busConfigurator.UsingInMemory(
                (context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        //Add options setups
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        //Add services
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthCookiesManager, AuthCookiesManager>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IGoalsManagerService, GoalsManagerService>();
        services.AddSingleton<INutritionCalculator, NutritionCalculator>();

        //Add repositories
        services.RegisterRepositories();

        //Add feature management
        services.AddFeatureManagement(configuration);

        return services;
    }
}
