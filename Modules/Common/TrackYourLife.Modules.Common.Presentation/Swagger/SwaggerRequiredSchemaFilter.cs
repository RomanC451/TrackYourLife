using NJsonSchema;
using NJsonSchema.Generation;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Common.Presentation.Swagger;

public class SwaggerRequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;

        if (schema.Properties == null)
            return;

        foreach (var schemaProp in schema.Properties)
        {
            if (schemaProp.Value.IsNullableRaw == true)
                continue;

            if (!schema.RequiredProperties.Contains(schemaProp.Key))
                schema.RequiredProperties.Add(schemaProp.Key);
        }
    }
}
