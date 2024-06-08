using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.Common.Application.Core.Abstractions.Services;

namespace TrackYourLife.Common.Infrastructure.Health;

public class FoodApiServiceHealthCheck(IFoodApiService foodApiService) : IHealthCheck
{
    private readonly IFoodApiService _foodApiService = foodApiService;

    private const string egFoodName = "lapte";

    private const int page = 1;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default
    )
    {
        var result = await _foodApiService.GetFoodListAsync(egFoodName, page, cancellationToken);

        if (result.IsFailure)
            return HealthCheckResult.Unhealthy(description: result.Error.ToString());

        return HealthCheckResult.Healthy();
    }
}
