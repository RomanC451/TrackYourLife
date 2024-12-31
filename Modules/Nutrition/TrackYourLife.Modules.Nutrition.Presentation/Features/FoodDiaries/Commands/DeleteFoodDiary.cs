using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

internal sealed class DeleteFoodDiary(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<FoodDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new DeleteFoodDiaryCommand(Route<NutritionDiaryId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
