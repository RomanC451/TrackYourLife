using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.ServingSizes;

[Collection("NutritionRepositoryTests")]
public class ServingSizeRepositoryTests : BaseRepositoryTests
{
    private ServingSizeRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new ServingSizeRepository(_writeDbContext!);
    }

    [Fact]
    public async Task GetByApiIdAsync_WhenServingSizeExists_ShouldReturnServingSize()
    {
        // Arrange
        var apiId = 123456L;
        var servingSize = ServingSizeFaker.Generate(apiId: apiId);
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByApiIdAsync(apiId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(servingSize.Id);
            result.NutritionMultiplier.Should().Be(servingSize.NutritionMultiplier);
            result.Unit.Should().Be(servingSize.Unit);
            result.Value.Should().Be(servingSize.Value);
            result.ApiId.Should().Be(apiId);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByApiIdAsync_WhenServingSizeDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentApiId = 999999L;

        try
        {
            // Act
            var result = await _sut.GetByApiIdAsync(nonExistentApiId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetWhereApiIdPartOfListAsync_WhenServingSizesExist_ShouldReturnMatchingServingSizes()
    {
        // Arrange
        var apiIds = new List<long> { 123456L, 789012L };
        var servingSize1 = ServingSizeFaker.Generate(apiId: apiIds[0]);
        var servingSize2 = ServingSizeFaker.Generate(apiId: apiIds[1]);
        var nonMatchingServingSize = ServingSizeFaker.Generate(apiId: 999999L);

        await _writeDbContext!.ServingSizes.AddRangeAsync(
            servingSize1,
            servingSize2,
            nonMatchingServingSize
        );
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetWhereApiIdPartOfListAsync(apiIds, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(ss => ss.Id == servingSize1.Id);
            result.Should().Contain(ss => ss.Id == servingSize2.Id);
            result.Should().NotContain(ss => ss.Id == nonMatchingServingSize.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetWhereApiIdPartOfListAsync_WhenNoMatchingServingSizes_ShouldReturnEmptyList()
    {
        // Arrange
        var nonMatchingApiIds = new List<long> { 999999L, 888888L };
        var servingSize = ServingSizeFaker.Generate(apiId: 123456L);

        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetWhereApiIdPartOfListAsync(
                nonMatchingApiIds,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
