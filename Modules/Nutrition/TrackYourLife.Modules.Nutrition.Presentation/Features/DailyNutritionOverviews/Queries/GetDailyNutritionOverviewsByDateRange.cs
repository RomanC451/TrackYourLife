using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews.Queries;

internal sealed record GetDailyNutritionOverviewsByDateRangeRequest(
    DateOnly StartDate,
    DateOnly EndDate
)
{
    [QueryParam]
    public DateOnly StartDate { get; init; } = StartDate;

    [QueryParam]
    public DateOnly EndDate { get; init; } = EndDate;
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
            .Create(new GetDailyNutritionOverviewsByDateRangeQuery(req.StartDate, req.EndDate))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(overviews => overviews.Select(o => o.ToDto()));
    }
}
