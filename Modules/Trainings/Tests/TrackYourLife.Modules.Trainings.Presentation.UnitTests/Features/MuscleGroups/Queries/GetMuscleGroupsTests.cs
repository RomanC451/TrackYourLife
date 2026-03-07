using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Trainings.Application.Features.MuscleGroups.Queries.GetMuscleGroups;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.Modules.Trainings.Presentation.Features.MuscleGroups.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Presentation.UnitTests.Features.MuscleGroups.Queries;

public class GetMuscleGroupsTests
{
    private readonly ISender _sender;
    private readonly GetMuscleGroups _endpoint;

    public GetMuscleGroupsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetMuscleGroups(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithMuscleGroupTree()
    {
        var chestId = new MuscleGroupId(Guid.NewGuid());
        var expectedDto = new List<MuscleGroupDto>
        {
            new(chestId, "Chest", null, new List<MuscleGroupDto>())
        };

        _sender
            .Send(Arg.Any<GetMuscleGroupsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IReadOnlyList<MuscleGroupDto>>(expectedDto)));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IReadOnlyList<MuscleGroupDto>>>().Subject;
        okResult.Value.Should().NotBeNull().And.HaveCount(1);
        var list = okResult.Value!;
        list[0].Name.Should().Be("Chest");
        list[0].Id.Should().Be(chestId);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetMuscleGroupsQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceedsWithEmptyList_ShouldReturnOkWithEmptyList()
    {
        _sender
            .Send(Arg.Any<GetMuscleGroupsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IReadOnlyList<MuscleGroupDto>>(new List<MuscleGroupDto>())));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IReadOnlyList<MuscleGroupDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().BeEmpty();
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("Error", "Failed to get muscle groups");
        _sender
            .Send(Arg.Any<GetMuscleGroupsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IReadOnlyList<MuscleGroupDto>>(error)));

        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
