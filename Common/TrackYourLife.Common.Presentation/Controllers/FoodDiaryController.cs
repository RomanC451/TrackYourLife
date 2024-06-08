using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using TrackYourLife.Application.FoodDiaries.Commands.AddFoodDiaryEntry;
using TrackYourLife.Application.FoodDiaries.Commands.UpdateFoodDiaryEntry;
using TrackYourLife.Application.FoodDiaries.Queries.GetFoodDiaryByDate;
using TrackYourLife.Application.FoodDiaries.Queries.GetTotalCaloriesByPeriod;
using TrackYourLife.Application.FoodDiaries.Commands.RemoveFoodDiaryEntry;
using TrackYourLife.Contracts.FoodDiaries;
using TrackYourLife.Domain.Errors;
using TrackYourLife.Domain.FoodDiaries;
using TrackYourLife.Domain.Shared;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Contracts.Common;
using TrackYourLife.Common.Presentation.Contracts;
using TrackYourLife.Common.Presentation.Abstractions;

namespace TrackYourLife.Common.Presentation.Controllers;

public class FoodDiaryController(ISender sender, IMapper mapper, IFeatureManager featureManager) : ApiController(sender, mapper, featureManager)
{
    [HttpGet(ApiRoutes.FoodDiary.GetByDate)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.FoodDiary.GetByDate))]
    [ProducesResponseType(typeof(FoodDiaryEntryListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDateAsync(
        [FromQuery] GetFoodDiaryByDateRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<GetFoodDiaryByDateQuery>)
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [Authorize]
    [HttpPost(ApiRoutes.FoodDiary.AddEntry)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.FoodDiary.AddEntry))]
    [ProducesResponseType(typeof(IdResponse), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddEntryAsync(
        AddFoodDiaryEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<AddFoodDiaryEntryCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [Authorize]
    [HttpPut(ApiRoutes.FoodDiary.UpdateEntry)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.FoodDiary.UpdateEntry))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> UpdateEntryAsync(
        UpdateFoodDiaryEntryRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<UpdateFoodDiaryEntryCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [Authorize]
    [HttpDelete(ApiRoutes.FoodDiary.DeleteEntry)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.FoodDiary.DeleteEntry))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteEntryAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Result
            .Create(new RemoveFoodDiaryEntryCommand(FoodDiaryEntryId.Create(id)))
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [Authorize]
    [HttpGet(ApiRoutes.FoodDiary.GetTotalCalories)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.FoodDiary.GetTotalCalories))]
    [ProducesResponseType(typeof(TotalCaloriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTotalCaloriesByPeriodAsync(
        [FromQuery] GetTotalCaloriesByPeriodRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<GetTotalCaloriesByPeriodQuery>)
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }
}
