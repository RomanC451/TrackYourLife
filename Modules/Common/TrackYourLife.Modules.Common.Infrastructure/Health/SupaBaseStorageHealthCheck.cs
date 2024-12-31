using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.Modules.Common.Application.Core.Abstractions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Common.Infrastructure.Health;

/// <summary>
/// Represents a health check for the SupaBase client.
/// </summary>
public class SupaBaseStorageHealthCheck(ISupaBaseStorage supaBaseClient) : IHealthCheck
{
    private const string bucketName = "FoodApi";

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
        var result = await supaBaseClient.GetAllFilesNamesFromBucketAsync(bucketName, true);

        if (result.IsFailure)
            return HealthCheckResult.Unhealthy(description: result.Error.ToString());

        return HealthCheckResult.Healthy();
    }
}
