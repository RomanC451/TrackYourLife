using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackYourLife.Modules.Common.Presentation.Swagger;

namespace TrackYourLife.Modules.Common.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddCommonPresentationServices(
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
                        // TODO: Change to the actual client URL before deployment
                        .SetIsOriginAllowed(origin => true);
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
            .AddFastEndpoints()
            .ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .SwaggerDocument(o =>
            {
                o.ShortSchemaNames = true;
                o.AutoTagPathSegmentIndex = 2;

                o.DocumentSettings = s =>
                {
                    s.SchemaSettings.SchemaProcessors.Add(new SwaggerRequiredSchemaProcessor());
                    s.SchemaSettings.SchemaProcessors.Add(new IdResponseSchemaFilter());

                    // var strongTypedIdTypes = AppDomain
                    //     .CurrentDomain.GetAssemblies()
                    //     .SelectMany(a => a.GetTypes())
                    //     .Where(t => t.GetInterfaces().Contains(typeof(IStronglyTypedGuid)));
                    // // .Where(t => t.Name.Contains("ExerciseId"));

                    // foreach (var type in strongTypedIdTypes)
                    // {
                    //     var idSchema = new JsonSchema
                    //     {
                    //         Type = JsonObjectType.String,
                    //         Format = "guid",
                    //         IsNullableRaw = false,
                    //     };

                    //     s.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(type, idSchema));
                    // }
                };
            });

        return services;
    }
}
