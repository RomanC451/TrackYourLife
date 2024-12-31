using Microsoft.Extensions.Diagnostics.HealthChecks;
using TrackYourLife.Modules.Common.Infrastructure.Health;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Common.Infrastructure.UnitTests.Health;

public class SupaBaseStorageHealthCheckTest : IDisposable
{
    private readonly SupaBaseStorageHealthCheck sut;
    private readonly ISupaBaseStorage supaBaseClient;

    public SupaBaseStorageHealthCheckTest()
    {
        supaBaseClient = Substitute.For<ISupaBaseStorage>();
        sut = new SupaBaseStorageHealthCheck(supaBaseClient);
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
        supaBaseClient.GetAllFilesNamesFromBucketAsync("FoodApi", true).Returns(failureResult);

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Unhealthy);
        result.Description.Should().Be(failureResult.Error.ToString());
    }

    [Fact]
    public async Task CheckHealthAsync_WhenClientReturnsSuccess_ShouldReturnHealthy()
    {
        // Arrange
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;
        var successResult = Result.Success<IEnumerable<string>>(["file1", "file2"]);
        supaBaseClient.GetAllFilesNamesFromBucketAsync("FoodApi", true).Returns(successResult);

        // Act
        var result = await sut.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        supaBaseClient.ClearSubstitute();
    }
}
