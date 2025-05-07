using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Queries;

internal sealed class GetFoodById(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{id}");
        Group<FoodsGroup>();
        Description(x =>
            x.Produces<FoodDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetFoodByIdQuery(Route<FoodId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(food => food.ToDto());
    }
}
