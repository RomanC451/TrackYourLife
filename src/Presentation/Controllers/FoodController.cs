using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Application.Foods.Queries.GetFoodById;
using TrackYourLifeDotnet.Application.Foods.Queries.GetFoodList;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Abstractions;

namespace TrackYourLifeDotnet.Presentation.Controllers;

[Route("api/[controller]")]
public class FoodController : ApiController
{
    public FoodController(ISender sender, IMapper mapper)
        : base(sender, mapper) { }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> GetFoodListAsync(
        [FromQuery] GetFoodListRequest request,
        CancellationToken cancellationToken
    )
    {
        GetFoodListQuery query = _mapper.Map<GetFoodListQuery>(request);

        Result<GetFoodListResult> result = await _sender.Send(query, cancellationToken);

        return MatchResponse<GetFoodListResult, GetFoodListResponse>(result);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFoodAsync(Guid id, CancellationToken cancellationToken)
    {
        GetFoodByIdQuery query = new(new FoodId(id));

        Result<GetFoodByIdResult> result = await _sender.Send(query, cancellationToken);

        return MatchResponse<GetFoodByIdResult, GetFoodByIdResponse>(result);
    }
}
