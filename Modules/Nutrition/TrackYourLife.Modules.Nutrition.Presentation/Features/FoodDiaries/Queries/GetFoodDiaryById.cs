using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Queries;

public class GetFoodDiaryById(ISender sender, INutritionMapper mapper)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{id}");
        Group<FoodDiariesGroup>();
        Description(x =>
            x.Produces<FoodDiaryDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetFoodDiaryByIdQuery(Route<NutritionDiaryId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(diary => TypedResults.Ok(mapper.Map<FoodDiaryDto>(diary)));
    }
}
