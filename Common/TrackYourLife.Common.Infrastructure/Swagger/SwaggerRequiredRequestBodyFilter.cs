using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TrackYourLife.Common.Infrastructure.Swagger;

public class SwaggerRequiredRequestBodyFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody != null)
        {
            operation.RequestBody.Required = true;
        }
    }
}
