using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.Modules.Common.Infrastructure.Health;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Health;

public class SupaBaseStorageHealthCheckTests
{
    private readonly ISupaBaseStorage _supaBaseClient;
    private readonly SupaBaseStorageHealthCheck _sut;

    public SupaBaseStorageHealthCheckTests()
    {
        _supaBaseClient = Substitute.For<ISupaBaseStorage>();
        _sut = new SupaBaseStorageHealthCheck(_supaBaseClient);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenClientReturnsSuccess_ShouldReturnHealthy()
    {
        // Arrange
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;
        var successResult = Result.Success<IEnumerable<string>>(["file1", "file2"]);
        _supaBaseClient.GetAllFilesNamesFromBucketAsync("FoodApi", true).Returns(successResult);

        // Act
        var result = await _sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenClientReturnsFailure_ShouldReturnUnhealthy()
    {
        // Arrange
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;
        var failureResult = Result.Failure<IEnumerable<string>>(
            InfrastructureErrors.SupaBaseClient.ClientNotWorking
        );
        _supaBaseClient.GetAllFilesNamesFromBucketAsync("FoodApi", true).Returns(failureResult);

        // Act
        var result = await _sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be(failureResult.Error.ToString());
    }

    [Fact]
    public async Task CheckHealthAsync_WhenClientReturnsNoFiles_ShouldReturnUnhealthy()
    {
        // Arrange
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;
        var noFilesResult = Result.Failure<IEnumerable<string>>(
            InfrastructureErrors.SupaBaseClient.NoFilesInBucket
        );
        _supaBaseClient.GetAllFilesNamesFromBucketAsync("FoodApi", true).Returns(noFilesResult);

        // Act
        var result = await _sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be(noFilesResult.Error.ToString());
    }
}
