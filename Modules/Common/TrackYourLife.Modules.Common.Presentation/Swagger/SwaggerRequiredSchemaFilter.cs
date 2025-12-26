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
        var type = context.ContextualType?.TypeInfo;

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

        // Handle ExerciseSetChange derived types
        if (
            type?.FullName?.Contains("ExerciseSetChange") == true
            && type.FullName
                != "TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories.ExerciseSetChange"
        )
        {
            // For WeightBasedExerciseSetChange
            if (type.FullName.Contains("WeightBasedExerciseSetChange"))
            {
                EnsurePropertyRequired(schema, "weightChange");
                EnsurePropertyRequired(schema, "repsChange");
            }
            // For TimeBasedExerciseSetChange
            else if (type.FullName.Contains("TimeBasedExerciseSetChange"))
            {
                EnsurePropertyRequired(schema, "durationChange");
            }
            // For BodyweightExerciseSetChange
            else if (type.FullName.Contains("BodyweightExerciseSetChange"))
            {
                EnsurePropertyRequired(schema, "repsChange");
            }
            // For DistanceExerciseSetChange
            else if (type.FullName.Contains("DistanceExerciseSetChange"))
            {
                EnsurePropertyRequired(schema, "distanceChange");
            }
            // For CustomExerciseSetChange
            else if (type.FullName.Contains("CustomExerciseSetChange"))
            {
                EnsurePropertyRequired(schema, "valueChange");
            }
        }

        // Apply the general rule for all properties
        foreach (var schemaProp in schema.Properties)
        {
            if (schemaProp.Value.IsNullableRaw == true)
                continue;

            if (!schema.RequiredProperties.Contains(schemaProp.Key))
                schema.RequiredProperties.Add(schemaProp.Key);
        }
    }

    private static void EnsurePropertyRequired(JsonSchema schema, string propertyName)
    {
        if (schema.Properties.ContainsKey(propertyName))
        {
            System.Diagnostics.Debug.WriteLine($"Making property {propertyName} required");
            schema.Properties[propertyName].IsNullableRaw = false;
            if (!schema.RequiredProperties.Contains(propertyName))
            {
                schema.RequiredProperties.Add(propertyName);
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Property {propertyName} not found in schema");
        }
    }
}
