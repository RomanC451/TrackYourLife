using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Infrastructure.Swagger;

public static class SwaggerGenConfiguration
{
    public static void AddCustomSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc(
                "v1",
                new OpenApiInfo { Title = "EventReminder API", Version = "v1" }
            );

            var strongTypedIdTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(typeof(IStronglyTypedGuid)));

            foreach (var type in strongTypedIdTypes)
            {
                swaggerGenOptions.MapType(
                    type,
                    () => new OpenApiSchema { Type = "string", Format = "uuid" }
                );
            }

            swaggerGenOptions.SchemaFilter<IdResponseSchemaFilter>();
            swaggerGenOptions.SchemaFilter<SwaggerRequiredSchemaFilter>();
            swaggerGenOptions.OperationFilter<SwaggerRequiredParametersFilter>();
            swaggerGenOptions.OperationFilter<SwaggerRequiredRequestBodyFilter>();

            swaggerGenOptions.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                }
            );

            swaggerGenOptions.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );

            swaggerGenOptions.SupportNonNullableReferenceTypes();

            swaggerGenOptions.EnableAnnotations();
        });
    }
}
