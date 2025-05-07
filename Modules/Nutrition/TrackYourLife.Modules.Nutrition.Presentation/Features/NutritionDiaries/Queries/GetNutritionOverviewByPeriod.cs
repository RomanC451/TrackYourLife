using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;

internal sealed record GetNutritionOverviewByPeriodRequest(DateOnly StartDate, DateOnly EndDate)
{
    [QueryParam]
    public DateOnly StartDate { get; init; } = StartDate;

    [QueryParam]
    public DateOnly EndDate { get; init; } = EndDate;
}

internal sealed class GetNutritionOverviewByPeriod(ISender sender)
    : Endpoint<GetNutritionOverviewByPeriodRequest, IResult>
{
    public override void Configure()
    {
        Get("nutrition-overview");
        Group<NutritionDiariesGroup>();
        Description(x => x.Produces<NutritionalContent>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(
        GetNutritionOverviewByPeriodRequest req,
        CancellationToken ct
    )
    {
        return await sender
            .Send(new GetNutritionOverviewByPeriodQuery(req.StartDate, req.EndDate), ct)
            .ToActionResultAsync(total => total);
    }
}
