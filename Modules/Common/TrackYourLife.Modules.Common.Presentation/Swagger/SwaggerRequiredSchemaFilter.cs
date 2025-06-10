using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.Generation;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Common.Presentation.Swagger;

[JsonSchema]
public class SwaggerRequiredSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        var schema = context.Schema;

        if (schema.Properties == null)
            return;

        // Temporary debug check
        if (
            context.ContextualType?.TypeInfo?.FullName?.Contains("GetExerciseHistoryByExerciseId")
            == true
        )
        {
            // System.Diagnostics.Debugger.Break();
            schema.Properties["exerciseId"].IsNullableRaw = false;
        }

        foreach (var schemaProp in schema.Properties)
        {
            if (schemaProp.Value.IsNullableRaw == true)
                continue;

            if (!schema.RequiredProperties.Contains(schemaProp.Key))
                schema.RequiredProperties.Add(schemaProp.Key);
        }
    }
}
