using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.Annotations;
using TrackYourLife.Application.Foods.Queries.GetFoodById;
using TrackYourLife.Application.Foods.Queries.GetFoodList;
using TrackYourLife.Common.Presentation.Abstractions;
using TrackYourLife.Common.Presentation.Contracts;
using TrackYourLife.Contracts.Foods;
using TrackYourLife.Domain.Errors;
using TrackYourLife.Domain.Foods;
using TrackYourLife.Domain.Shared;

namespace TrackYourLife.Common.Presentation.Controllers;

public class FoodController(ISender sender, IMapper mapper, IFeatureManager featureManager) : ApiController(sender, mapper, featureManager)
{
    [HttpGet(ApiRoutes.Food.GetList)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Food.GetList))]
    [ProducesResponseType(typeof(FoodListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListAsync(
        [FromQuery] GetFoodListRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<GetFoodListQuery>)
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [Authorize]
    [HttpGet(ApiRoutes.Food.GetById)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Food.GetById))]
    [ProducesResponseType(typeof(FoodResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFoodAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Result
            .Create(new GetFoodByIdQuery(new FoodId(id)), DomainErrors.General.UnProcessableRequest)
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }
}
