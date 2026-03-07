using TrackYourLife.Modules.Trainings.Application.Features.MuscleGroups.Queries.GetMuscleGroups;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.MuscleGroups.Queries.GetMuscleGroups;

public class GetMuscleGroupsQueryHandlerTests
{
    private readonly IMuscleGroupsQuery _muscleGroupsQuery;
    private readonly GetMuscleGroupsQueryHandler _handler;

    public GetMuscleGroupsQueryHandlerTests()
    {
        _muscleGroupsQuery = Substitute.For<IMuscleGroupsQuery>();
        _handler = new GetMuscleGroupsQueryHandler(_muscleGroupsQuery);
    }

    [Fact]
    public async Task Handle_WhenNoMuscleGroups_ShouldReturnEmptyTree()
    {
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<MuscleGroup>());

        var result = await _handler.Handle(new GetMuscleGroupsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
        await _muscleGroupsQuery.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOnlyRootGroups_ShouldReturnFlatList()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var backId = new MuscleGroupId(Guid.NewGuid());
        var muscleGroups = new List<MuscleGroup>
        {
            MuscleGroup.Create(backId, "Back", null),
            MuscleGroup.Create(chestId, "Chest", null),
        };
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(muscleGroups);

        var result = await _handler.Handle(new GetMuscleGroupsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(d => d.Name).Should().Contain("Back");
        result.Value.Select(d => d.Name).Should().Contain("Chest");
        result.Value.Should().OnlyContain(d => d.Children.Count == 0);
    }

    [Fact]
    public async Task Handle_WhenHierarchyExists_ShouldReturnTreeWithChildren()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var upperChestId = new MuscleGroupId(Guid.NewGuid());
        var lowerChestId = new MuscleGroupId(Guid.NewGuid());
        var muscleGroups = new List<MuscleGroup>
        {
            MuscleGroup.Create(chestId, "Chest", null),
            MuscleGroup.Create(upperChestId, "Upper Chest", chestId),
            MuscleGroup.Create(lowerChestId, "Lower Chest", chestId),
        };
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(muscleGroups);

        var result = await _handler.Handle(new GetMuscleGroupsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        var chestDto = result.Value.Single(d => d.Name == "Chest");
        chestDto.Id.Should().Be(chestId);
        chestDto.ParentMuscleGroupId.Should().BeNull();
        chestDto.Children.Should().HaveCount(2);
        chestDto.Children.Select(c => c.Name).Should().Contain("Upper Chest");
        chestDto.Children.Select(c => c.Name).Should().Contain("Lower Chest");
        chestDto.Children.Should().OnlyContain(c => c.ParentMuscleGroupId == chestId);
        chestDto.Children.Should().OnlyContain(c => c.Children.Count == 0);
    }

    [Fact]
    public async Task Handle_WhenMultipleRootsWithChildren_ShouldReturnCorrectTrees()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var upperChestId = new MuscleGroupId(Guid.NewGuid());
        var backId = new MuscleGroupId(Guid.NewGuid());
        var muscleGroups = new List<MuscleGroup>
        {
            MuscleGroup.Create(backId, "Back", null),
            MuscleGroup.Create(chestId, "Chest", null),
            MuscleGroup.Create(upperChestId, "Upper Chest", chestId),
        };
        _muscleGroupsQuery.GetAllAsync(Arg.Any<CancellationToken>()).Returns(muscleGroups);

        var result = await _handler.Handle(new GetMuscleGroupsQuery(), default);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2); // Back, Chest (roots)
        var chestDto = result.Value.Single(d => d.Name == "Chest");
        chestDto.Children.Should().HaveCount(1);
        chestDto.Children[0].Name.Should().Be("Upper Chest");
        var backDto = result.Value.Single(d => d.Name == "Back");
        backDto.Children.Should().BeEmpty();
    }
}
