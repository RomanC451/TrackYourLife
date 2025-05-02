using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Common.Infrastructure.Health;

/// <summary>
/// Represents a health check for the SupaBase client.
/// </summary>
internal sealed class SupaBaseStorageHealthCheck(ISupaBaseStorage supaBaseClient) : IHealthCheck
{
    // private const string bucketName = "food-api";

    /// <summary>
    /// Checks the health of the SupaBase client.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous health check operation.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var result = await supaBaseClient.GetAllFilesNamesFromBucketAsync("food-api", true);

        if (result.IsFailure)
            return HealthCheckResult.Unhealthy(description: result.Error.ToString());

        return HealthCheckResult.Healthy();
    }
}
