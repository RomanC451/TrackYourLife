using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TrackYourLife.Common.Infrastructure.Swagger;

public class SwaggerRequiredSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
            return;

        foreach (var schemaProp in schema.Properties)
        {
            if (schemaProp.Value.Nullable)
                continue;

            if (!schema.Required.Contains(schemaProp.Key))
                schema.Required.Add(schemaProp.Key);
        }
    }
}
