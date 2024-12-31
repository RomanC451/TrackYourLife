using System.ComponentModel;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Queries;

internal sealed record SearchFoodsByNameRequest
{
    [QueryParam]
    public string SearchParam { get; init; } = string.Empty;

    [QueryParam, DefaultValue(1)]
    public int? Page { get; init; }

    [QueryParam, DefaultValue(10)]
    public int? PageSize { get; init; }
}

internal sealed class SearchFoodsByName(ISender sender, INutritionMapper mapper)
    : Endpoint<SearchFoodsByNameRequest, IResult>
{
    public override void Configure()
    {
        Get("search");
        Group<FoodsGroup>();
        Description(x =>
            x.Produces<PagedList<FoodDto>>(200)
                .ProducesProblemFE<ProblemDetails>(404)
                .ProducesProblemFE<ProblemDetails>(400)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SearchFoodsByNameRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(mapper.Map<SearchFoodsByNameQuery>(req))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(pagedList => TypedResults.Ok(pagedList.Map(mapper.Map<FoodDto>)));
    }
}
