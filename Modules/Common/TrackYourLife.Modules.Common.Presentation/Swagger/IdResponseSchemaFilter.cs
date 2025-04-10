using NJsonSchema;
using NJsonSchema.Generation;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Common.Presentation.Swagger;

public class IdResponseSchemaFilter : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        if (context.ContextualType.Type == typeof(IdResponse))
        {
            context.Schema.Properties["id"] = new JsonSchemaProperty
            {
                Type = JsonObjectType.String,
                Format = "uuid",
            };
        }
    }
}
