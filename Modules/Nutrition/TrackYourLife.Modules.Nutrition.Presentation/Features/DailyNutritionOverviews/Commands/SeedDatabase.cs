using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Commands;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews.Commands;

public class SeedDatabase(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("");
        Group<DailyNutritionOverviewsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        await sender.Send(new SeedDatabaseCommand(), ct);

        return TypedResults.Ok();
    }
}
