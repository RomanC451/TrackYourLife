using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews.Queries;

internal sealed record GetDailyNutritionOverviewsByDateRangeRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    OverviewType OverviewType,
    AggregationMode AggregationMode
)
{
    [QueryParam]
    public DateOnly StartDate { get; init; } = StartDate;

    [QueryParam]
    public DateOnly EndDate { get; init; } = EndDate;

    [QueryParam]
    public OverviewType OverviewType { get; init; } = OverviewType;

    [QueryParam]
    public AggregationMode AggregationMode { get; init; } = AggregationMode;
}

internal sealed class GetDailyNutritionOverviewsByDateRange(ISender sender)
    : Endpoint<GetDailyNutritionOverviewsByDateRangeRequest, IResult>
{
    public override void Configure()
    {
        Get("range");
        Group<DailyNutritionOverviewsGroup>();
        Description(x =>
            x.Produces<IEnumerable<DailyNutritionOverviewDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetDailyNutritionOverviewsByDateRangeRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new GetDailyNutritionOverviewsByDateRangeQuery(
                    req.StartDate,
                    req.EndDate,
                    req.OverviewType,
                    req.AggregationMode
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(overviews => overviews.ToList());
    }
}
