using System.ComponentModel;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Queries;

internal sealed record GetExercisePerformanceRequest
{
    [QueryParam]
    public DateTime? StartDate { get; init; }

    [QueryParam]
    public DateTime? EndDate { get; init; }

    [QueryParam]
    public ExerciseId? ExerciseId { get; init; }

    [QueryParam]
    public PerformanceCalculationMethod? CalculationMethod { get; init; }

    [QueryParam, DefaultValue(1)]
    public int Page { get; init; } = 1;

    [QueryParam, DefaultValue(10)]
    public int PageSize { get; init; } = 10;
}

internal sealed class GetExercisePerformance(ISender sender)
    : Endpoint<GetExercisePerformanceRequest, IResult>
{
    public override void Configure()
    {
        Get("performance");
        Group<ExercisesHistoriesGroup>();
        Description(x =>
            x.Produces<PagedList<ExercisePerformanceDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetExercisePerformanceRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetExercisePerformanceQuery(
                    req.StartDate,
                    req.EndDate,
                    req.ExerciseId,
                    req.CalculationMethod ?? PerformanceCalculationMethod.Sequential,
                    req.Page,
                    req.PageSize
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(pagedList => pagedList);
    }
}
