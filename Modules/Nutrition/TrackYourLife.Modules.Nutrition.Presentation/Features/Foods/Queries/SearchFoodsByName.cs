using System.ComponentModel;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Queries;

internal sealed record SearchFoodsByNameRequest
{
    [QueryParam, DefaultValue("")]
    public string SearchParam { get; init; } = string.Empty;

    [QueryParam, DefaultValue(1)]
    public int Page { get; init; } = 1;

    [QueryParam, DefaultValue(10)]
    public int PageSize { get; init; } = 10;
}

internal sealed class SearchFoodsByName(ISender sender)
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
            .Create(new SearchFoodsByNameQuery(req.SearchParam, req.Page, req.PageSize))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(pagedList => pagedList);
    }
}
