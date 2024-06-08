using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using AspNetCoreRateLimit;
using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Common.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentationServices(
        this IServiceCollection services,
        IConfiguration Configuration
    )
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "CORSPolicy",
                builder =>
                {
                    builder
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("https://localhost:5173");
                }
            );
        });

        services.AddHttpContextAccessor();

        services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));
        services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddApplicationPart(AssemblyReference.Assembly);

        return services;
    }
}
