using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetTotalCaloriesByPeriod;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;

internal sealed record GetTotalCaloriesByPeriodRequest(DateOnly StartDate, DateOnly EndDate)
{
    [QueryParam]
    public DateOnly StartDate { get; init; } = StartDate;

    [QueryParam]
    public DateOnly EndDate { get; init; } = EndDate;
}

internal sealed class GetTotalCaloriesByPeriod(ISender sender)
    : Endpoint<GetTotalCaloriesByPeriodRequest, IResult>
{
    public override void Configure()
    {
        Get("total-calories");
        Group<NutritionDiariesGroup>();
        Description(x => x.Produces<int>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(
        GetTotalCaloriesByPeriodRequest req,
        CancellationToken ct
    )
    {
        return await sender
            .Send(new GetTotalCaloriesByPeriodQuery(req.StartDate, req.EndDate), ct)
            .ToActionResultAsync(total => TypedResults.Ok(total));
    }
}
