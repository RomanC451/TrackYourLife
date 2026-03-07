using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.MuscleGroups;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.MuscleGroups;

public class MuscleGroupsQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private MuscleGroupsQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new MuscleGroupsQuery(WriteDbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoMuscleGroups_ShouldReturnEmptyList()
    {
        try
        {
            var result = await _sut.GetAllAsync(CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenMuscleGroupsExist_ShouldReturnOrderedByName()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var backId = new MuscleGroupId(Guid.NewGuid());
        var tricepsId = new MuscleGroupId(Guid.NewGuid());
        var chest = MuscleGroup.Create(chestId, "Chest", null);
        var back = MuscleGroup.Create(backId, "Back", null);
        var triceps = MuscleGroup.Create(tricepsId, "Triceps", null);

        await WriteDbContext.MuscleGroups.AddAsync(chest);
        await WriteDbContext.MuscleGroups.AddAsync(back);
        await WriteDbContext.MuscleGroups.AddAsync(triceps);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            var result = await _sut.GetAllAsync(CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(m => m.Name).Should().ContainInOrder("Back", "Chest", "Triceps");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetAllAsync_WhenHierarchyExists_ShouldReturnAllInOrderByName()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var upperChestId = new MuscleGroupId(Guid.NewGuid());
        var lowerChestId = new MuscleGroupId(Guid.NewGuid());
        var chest = MuscleGroup.Create(chestId, "Chest", null);
        var upperChest = MuscleGroup.Create(upperChestId, "Upper Chest", chestId);
        var lowerChest = MuscleGroup.Create(lowerChestId, "Lower Chest", chestId);

        await WriteDbContext.MuscleGroups.AddAsync(chest);
        await WriteDbContext.MuscleGroups.AddAsync(upperChest);
        await WriteDbContext.MuscleGroups.AddAsync(lowerChest);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            var result = await _sut.GetAllAsync(CancellationToken.None);

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Select(m => m.Name).Should().ContainInOrder("Chest", "Lower Chest", "Upper Chest");
            var upper = result.Single(m => m.Name == "Upper Chest");
            upper.ParentMuscleGroupId.Should().Be(chestId);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
