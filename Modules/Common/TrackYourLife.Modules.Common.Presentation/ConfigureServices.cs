using System.Text.Json.Serialization;
using AspNetCoreRateLimit;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using TrackYourLife.Modules.Common.Presentation.Swagger;
using TrackYourLife.SharedLib.Domain.Ids;

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
                    s.DocumentProcessors.Add(new DocProcessor());
                    s.SchemaSettings.SchemaProcessors.Add(new SwaggerRequiredSchemaProcessor());
                    s.SchemaSettings.SchemaProcessors.Add(new IdResponseSchemaFilter());

                    var idSchema = new JsonSchema { Type = JsonObjectType.String, Format = "guid" };

                    var strongTypedIdTypes = AppDomain
                        .CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .Where(t => t.GetInterfaces().Contains(typeof(IStronglyTypedGuid)));

                    foreach (var type in strongTypedIdTypes)
                    {
                        s.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(type, idSchema));
                    }
                };
            });

        return services;
    }
}

sealed class DocProcessor : IDocumentProcessor
{
    public void Process(DocumentProcessorContext context)
    {
        context.Document.Servers.Add(new() { Url = "http://192.168.1.12:5001/" });
    }
}
