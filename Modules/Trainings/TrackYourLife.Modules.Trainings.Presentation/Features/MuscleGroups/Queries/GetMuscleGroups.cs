using TrackYourLife.Modules.Trainings.Application.Features.MuscleGroups.Queries.GetMuscleGroups;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.MuscleGroups.Queries;

public sealed class GetMuscleGroups(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<MuscleGroupsGroup>();
        Description(x => x.Produces<IReadOnlyList<MuscleGroupDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetMuscleGroupsQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}
