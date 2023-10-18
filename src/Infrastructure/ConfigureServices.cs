using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Quartz;

using Scrutor;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Infrastructure.Options;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;
using TrackYourLifeDotnet.Infrastructure.Services;
using TrackYourLifeDotnet.Infrastructure.BackgroundJobs;
using TrackYourLifeDotnet.Infrastructure.FluentValidation;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddValidatorsFromAssembly(
            TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly
        );

        // services.AddScoped<IValidator<JwtOptions>, JwtOptionsValidator>();

        services
            .AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.ConfigurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();

        services
            .AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.ConfigurationSection)
            .ValidateFluentValidation()
            .ValidateOnStart();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();

        services.Scan(
            selector =>
                selector
                    .FromAssemblies(TrackYourLifeDotnet.Infrastructure.AssemblyReference.Assembly)
                    .AddClasses(
                        classes =>
                            classes.Where(
                                type =>
                                    type.Namespace != null
                                    && !type.Namespace.StartsWith(
                                        "TrackYourLifeDotnet.Infrastructure.FluentValidation"
                                    )
                            )
                    )
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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

        return services;
    }
}
