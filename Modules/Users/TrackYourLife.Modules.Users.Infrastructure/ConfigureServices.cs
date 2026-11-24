using FluentValidation;
using MailKit.Net.Smtp;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Goals.Consumers;
using TrackYourLife.Modules.Users.Application.Features.Users.Consumers;
using TrackYourLife.Modules.Users.Application.Features.Goals.Services;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.Modules.Users.Infrastructure.Authentication;
using TrackYourLife.Modules.Users.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Users.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Extensions;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.OptionsSetup;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Application.Abstraction;
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

        // Add Background jobs
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey($"{nameof(ProcessUsersOutboxMessagesJob)}");

            configure
                .AddJob<ProcessUsersOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(jobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10).RepeatForever()
                        )
                );
        });

        //Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

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
        services.RegisterConsumer<GetNutritionGoalsByUserIdConsumer>();
        services.RegisterConsumer<GetReadingGoalByUserIdConsumer>();
        services.RegisterConsumer<GetUserForBillingByIdConsumer>();
        services.RegisterConsumer<GetUserAccountByIdConsumer>();
        services.RegisterConsumer<SendYoutubeSettingsPasswordResetEmailConsumer>();
        services.RegisterConsumer<GetUserForBillingByStripeCustomerIdConsumer>();
        services.RegisterConsumer<UpgradeToProConsumer>();
        services.RegisterConsumer<DowngradeProConsumer>();
        services.RegisterConsumer<UpdateProSubscriptionPeriodEndConsumer>();
        services.RegisterConsumer<SetStripeCustomerIdConsumer>();

        //Add options setups
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Dual auth: JWT (web app) or personal access token (Home Assistant / integrations)
        services
            .AddAuthentication()
            .AddPolicyScheme(
                PersonalAccessTokenDefaults.SmartScheme,
                "JWT or Personal Access Token",
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        string? authorization = context.Request.Headers.Authorization;

                        if (
                            !string.IsNullOrEmpty(authorization)
                            && authorization.StartsWith(
                                "Bearer ",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        {
                            var token = authorization["Bearer ".Length..].Trim();

                            if (
                                token.StartsWith(
                                    PersonalAccessToken.TokenPrefixMarker,
                                    StringComparison.Ordinal
                                )
                            )
                            {
                                return PersonalAccessTokenDefaults.AuthenticationScheme;
                            }
                        }

                        return JwtBearerDefaults.AuthenticationScheme;
                    };
                }
            )
            .AddScheme<AuthenticationSchemeOptions, PersonalAccessTokenAuthenticationHandler>(
                PersonalAccessTokenDefaults.AuthenticationScheme,
                _ => { }
            );

        services.PostConfigure<AuthenticationOptions>(options =>
        {
            options.DefaultAuthenticateScheme = PersonalAccessTokenDefaults.SmartScheme;
            options.DefaultChallengeScheme = PersonalAccessTokenDefaults.SmartScheme;
            options.DefaultScheme = PersonalAccessTokenDefaults.SmartScheme;
        });

        //Add services
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IPersonalAccessTokenHasher, PersonalAccessTokenHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthCookiesManager, AuthCookiesManager>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IGoalsManagerService, GoalsManagerService>();
        services.AddSingleton<INutritionGoalsCalculator, NutritionGoalsCalculator>();

        //Add repositories
        services.RegisterRepositories();

        //Add feature management
        services.AddFeatureManagement<UsersFeatureFlags>(configuration);

        services.AddScoped<ISmtpClient, SmtpClient>();

        services.AddSingleton<IAuthorizationBlackListStorage, AuthorizationBlackListStorage>();

        services.AddScoped<ISubscriptionStatusProvider, SubscriptionStatusProvider>();

        return services;
    }
}
