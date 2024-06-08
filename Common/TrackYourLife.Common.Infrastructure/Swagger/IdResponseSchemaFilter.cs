using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TrackYourLife.Common.Contracts.Common;

namespace TrackYourLife.Common.Infrastructure.Swagger;

public class IdResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(IdResponse))
        {
            schema.Properties["id"] = new OpenApiSchema { Type = "string", Format = "uuid" };
        }
    }
}
