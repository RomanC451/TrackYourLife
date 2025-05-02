using System.Net.Mail;
using FluentValidation;
using MassTransit;
using MassTransit.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using TrackYourLife.Modules.Common.Application.Core.Abstraction;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Infrastructure.Authentication;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Common.Infrastructure.Health;
using TrackYourLife.Modules.Common.Infrastructure.Options;
using TrackYourLife.Modules.Common.Infrastructure.Services;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Infrastructure.Extensions;
using static TrackYourLife.SharedLib.Domain.Errors.InfrastructureErrors;

namespace TrackYourLife.Modules.Common.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        //Add db contexts
        services.AddDbContext<CommonWriteDbContext>();
        services.AddDbContext<CommonReadDbContext>();

        services.AddScoped<ICommonUnitOfWork, CommonUnitOfWork>();

        //Add repositories
        services.RegisterRepositories(AssemblyReference.Assembly);

        //Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        //Add options
        services.AddOptionsWithFluentValidation<SupaBaseOptions>(
            SupaBaseOptions.ConfigurationSection
        );

        //Add authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddAuthorization();

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddScoped<IUserIdentifierProvider, UserIdentifierProvider>();

        //Add background jobs
        services.AddQuartzHostedService();

        //Add caching
        services.AddMemoryCache();

        //Add SupaBase
        services.AddScoped<ISupabaseClient>(
            (provider) =>
            {
                var supaBaseOptions = provider
                    .GetRequiredService<IOptions<SupaBaseOptions>>()
                    .Value;

                return new Services.SupaBaseClient(supaBaseOptions.Url, supaBaseOptions.Key);
            }
        );

        services.AddScoped<ISupaBaseStorage, SupaBaseStorage>();

        //Add SMTP client
        services.AddTransient<SmtpClient, SmtpClient>();

        //Add health checks
        services.AddHealthChecks().AddNpgSql(configuration.GetConnectionString("Database") ?? "");
        services.AddHealthChecks().AddCheck<SupaBaseStorageHealthCheck>("SupaBaseStorage");

        //Add custom services
        services.AddScoped<ICookiesReader, CookiesReader>();

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.UsingInMemory(
                (context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        services.RegisterConsumer<AddCookiesFromFilesConsumer>();
        services.RegisterConsumer<GetCookiesByDomainsConsumer>();
        services.RegisterConsumer<AddCookiesConsumer>();

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Common"))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("Common")
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault().AddService(serviceName: "Common")
                    )
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddNpgsql()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(configuration["OtlpEndpoint"]!);
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                });
            });

        return services;
    }
}
