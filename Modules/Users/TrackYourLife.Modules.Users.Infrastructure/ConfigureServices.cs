using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.OutboxMessages;
using TrackYourLife.Modules.Users.Infrastructure.Authentication;
using TrackYourLife.Modules.Users.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Data.Outbox;
using TrackYourLife.Modules.Users.Infrastructure.Extensions;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.OptionsSetup;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Infrastructure.Data;
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

        //Add options setups
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        //Add services
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthCookiesManager, AuthCookiesManager>();
        services.AddScoped<IEmailService, EmailService>();

        //Add repositories
        services.RegisterRepositories();

        //Add feature management
        services.AddFeatureManagement(configuration);

        return services;
    }
}
