using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests;

public static class TestHelpers
{
    public static void SetHttpContext(this object endpoint, HttpContext httpContext)
    {
        var endpointType = endpoint.GetType();
        var baseType = endpointType.BaseType;

        // Try to find _httpContext field in the type hierarchy
        FieldInfo? httpContextField = null;
        while (baseType != null && httpContextField == null)
        {
            httpContextField = baseType.GetField(
                "_httpContext",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (httpContextField == null)
            {
                baseType = baseType.BaseType;
            }
        }

        // If not found in base types, try the endpoint type itself
        httpContextField ??= endpointType.GetField(
            "_httpContext",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (httpContextField != null)
        {
            httpContextField.SetValue(endpoint, httpContext);
        }
        else
        {
            // Try to find HttpContext property and set it via reflection
            var httpContextProperty = endpointType.GetProperty(
                "HttpContext",
                BindingFlags.Public | BindingFlags.Instance
            );
            if (httpContextProperty != null && httpContextProperty.CanWrite)
            {
                httpContextProperty.SetValue(endpoint, httpContext);
            }
        }
    }
}
