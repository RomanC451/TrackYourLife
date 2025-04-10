using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Health;

/// <summary>
/// Represents a health check implementation for the Food API service.
/// </summary>
internal sealed class FoodApiServiceHealthCheck(IFoodApiService foodApiService) : IHealthCheck
{
    private const string egFoodName = "lapte";

    /// <summary>
    /// Checks the health of the Food API service by searching for a food item and adding it to the database.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The health check result.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var result = await foodApiService.SearchFoodAndAddToDbAsync(egFoodName, cancellationToken);

        if (result.IsFailure)
            return HealthCheckResult.Unhealthy(description: result.Error.ToString());

        return HealthCheckResult.Healthy();
    }
}
