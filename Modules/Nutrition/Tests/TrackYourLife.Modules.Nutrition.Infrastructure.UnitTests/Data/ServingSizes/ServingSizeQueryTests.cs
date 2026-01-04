using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.ServingSizes;

public class ServingSizeQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private ServingSizeQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new ServingSizeQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetByIdAsync_WhenServingSizeExists_ShouldReturnServingSize()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(servingSize.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(servingSize.Id);
            result.NutritionMultiplier.Should().Be(servingSize.NutritionMultiplier);
            result.Unit.Should().Be(servingSize.Unit);
            result.Value.Should().Be(servingSize.Value);
            result.ApiId.Should().Be(servingSize.ApiId);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenServingSizeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = ServingSizeId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(nonExistentId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
