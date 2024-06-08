using System.ComponentModel.DataAnnotations;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.Annotations;
using TrackYourLife.Application.UserGoals.Commands.AddUserGoal;
using TrackYourLife.Application.UserGoals.Commands.UpdateUserGoal;
using TrackYourLife.Application.UserGoals.Queries.GetActiveUserGoalByType;
using TrackYourLife.Common.Presentation.Abstractions;
using TrackYourLife.Common.Presentation.Contracts;
using TrackYourLife.Contracts.UserGoals;
using TrackYourLife.Domain.Errors;
using TrackYourLife.Domain.Shared;
using TrackYourLife.Domain.UserGoals;

namespace TrackYourLife.Common.Presentation.Controllers;

public class UserGoalController(ISender sender, IMapper mapper, IFeatureManager featureManager) : ApiController(sender, mapper, featureManager)
{
    [HttpGet(ApiRoutes.UserGoal.GetActiveGoal)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.GetActiveGoal))]
    [ProducesResponseType(typeof(UserGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> GetActiveUserGoalByTypeAsync(
        [FromQuery, Required] UserGoalType GoalType,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(
                new GetActiveUserGoalByTypeQuery(GoalType),
                DomainErrors.General.UnProcessableRequest
            )
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }


    [HttpPost(ApiRoutes.UserGoal.AddGoal)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.AddGoal))]
    [ProducesResponseType(typeof(UserGoalResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUserGoalAsync(
        AddUserGoalRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<AddUserGoalCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [HttpPut(ApiRoutes.UserGoal.UpdateGoal)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.UpdateGoal))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]

    public async Task<IActionResult> UpdateUserGoalAsync(
        UpdateUserGoalRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<UpdateUserGoalCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }
}
