using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;

internal sealed record GetTopExercisesRequest
{
    [QueryParam, DefaultValue(1)]
    public int Page { get; init; } = 1;

    [QueryParam, DefaultValue(10)]
    public int PageSize { get; init; } = 10;

    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }
}

internal sealed class GetTopExercises(ISender sender) : Endpoint<GetTopExercisesRequest, IResult>
{
    public override void Configure()
    {
        Get("top");
        Group<ExercisesGroup>();
        Description(x =>
            x.Produces<PagedList<TopExerciseDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetTopExercisesRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetTopExercisesQuery(req.Page, req.PageSize, req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(pagedList => pagedList);
    }
}
