using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Application.FoodDiary.Commands.AddFoodDieryEntry;
using TrackYourLifeDotnet.Application.FoodDiary.Commands.RemoveFoodDiaryEntry;
using TrackYourLifeDotnet.Application.FoodDiary.Commands.UpdateFoodDiaryEntry;
using TrackYourLifeDotnet.Application.FoodDiary.Queries.GetFoodDiary;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Abstractions;

namespace TrackYourLifeDotnet.Presentation.Controllers;

[Route("api/[controller]")]
public class FoodDiaryController : ApiController
{
    public FoodDiaryController(ISender sender, IMapper mapper)
        : base(sender, mapper) { }

    [Authorize]
    [HttpGet("by-date")]
    public async Task<IActionResult> GetFoodDiaryAsync(
        [FromQuery] GetFoodDiaryRequest request,
        CancellationToken cancellationToken
    )
    {
        if (!DateOnly.TryParse(request.Date, out var date))
        {
            return BadRequest(new Error("Date.InvalidFormat", "Date has invalid format."));
        }

        GetFoodDiaryQuery query = new(date);

        Result<GetFoodDiaryResult> result = await _sender.Send(query, cancellationToken);

        return MatchResponse<GetFoodDiaryResult, GetFoodDiaryResponse>(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveFoodDiaryEntryAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        RemoveFoodDiaryEntryCommand command = new(new FoodDiaryEntryId(id));

        Result result = await _sender.Send(command, cancellationToken);

        return MatchResponse(result);
    }

    [Authorize]
    [HttpPost()]
    public async Task<IActionResult> AddFoodDiaryEntryAsync(
        [FromBody] AddFoodDiaryEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        AddFoodDiaryEntryCommand command = _mapper.Map<AddFoodDiaryEntryCommand>(request);

        Result<AddFoodDiaryEntryResult> result = await _sender.Send(command, cancellationToken);

        return MatchResponse<AddFoodDiaryEntryResult, AddFoodDiaryEntryResponse>(result);
    }

    [Authorize]
    [HttpPut()]
    public async Task<IActionResult> UpdateFoodDiaryEntryAsync(
        [FromBody] UpdateFoodDiaryEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        UpdateFoodDiaryEntryCommand command = _mapper.Map<UpdateFoodDiaryEntryCommand>(request);

        Result result = await _sender.Send(command, cancellationToken);

        return MatchResponse(result);
    }
}
