using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TrackYourLife.Common.Infrastructure.Swagger;

public class SwaggerRequiredParametersFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var correspondingApiParameter =
                context.ApiDescription.ParameterDescriptions.FirstOrDefault(
                    p => p.Name == parameter.Name
                );

            if (
                correspondingApiParameter != null
                && Nullable.GetUnderlyingType(correspondingApiParameter.ModelMetadata.ModelType)
                    == null
            )
            {
                parameter.Required = true;
            }
        }
    }
}
