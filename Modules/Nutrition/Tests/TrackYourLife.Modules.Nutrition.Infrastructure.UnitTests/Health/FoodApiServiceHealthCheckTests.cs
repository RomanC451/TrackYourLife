using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.Health;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Health;

public class FoodApiServiceHealthCheckTests : IDisposable
{
    private readonly IFoodApiService foodApiService = Substitute.For<IFoodApiService>();
    private readonly FoodApiServiceHealthCheck healthCheck;

    public FoodApiServiceHealthCheckTests()
    {
        healthCheck = new FoodApiServiceHealthCheck(foodApiService);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodApiService.ClearReceivedCalls();
    }

    [Fact]
    public async Task CheckHealthAsync_WhenServiceIsHealthy_ShouldReturnHealthy()
    {
        // Arrange
        foodApiService
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None
        );

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);

        await foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CheckHealthAsync_WhenServiceIsUnhealthy_ShouldReturnUnhealthy()
    {
        // Arrange
        foodApiService
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(new Error("Food.NotFound", "asdsad")));

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None
        );

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);

        await foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
