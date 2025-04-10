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
    private readonly IFoodApiService _foodApiService = Substitute.For<IFoodApiService>();
    private readonly FoodApiServiceHealthCheck _healthCheck;

    public FoodApiServiceHealthCheckTests()
    {
        _healthCheck = new FoodApiServiceHealthCheck(_foodApiService);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _foodApiService.ClearReceivedCalls();
    }

    [Fact]
    public async Task CheckHealthAsync_WhenServiceIsHealthy_ShouldReturnHealthy()
    {
        // Arrange
        _foodApiService
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None
        );

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
        await _foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CheckHealthAsync_WhenServiceIsUnhealthy_ShouldReturnUnhealthy()
    {
        // Arrange
        var error = new Error("Food.NotFound", "Food not found");
        _foodApiService
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        var result = await _healthCheck.CheckHealthAsync(
            new HealthCheckContext(),
            CancellationToken.None
        );

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be(error.ToString());
        await _foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
