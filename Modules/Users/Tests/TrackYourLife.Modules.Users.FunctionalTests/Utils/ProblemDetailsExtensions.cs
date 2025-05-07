using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TrackYourLife.Modules.Users.FunctionalTests.Utils;

public static class ProblemDetailsExtensions
{
    public record ErrorResponse(string Code, string Message, int HttpStatus);

    public static ErrorResponse[] GetExtensionErrors(this ProblemDetails problemDetails)
    {
        if (problemDetails.Extensions.TryGetValue("errors", out var errorElement))
        {
            return JsonSerializer.Deserialize<ErrorResponse[]>(
                    errorElement!.ToString()!,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? Array.Empty<ErrorResponse>();
        }

        return Array.Empty<ErrorResponse>();
    }
}
