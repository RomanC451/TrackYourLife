//using System.ComponentModel.DataAnnotations;
//using MapsterMapper;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.FeatureManagement;
//using Swashbuckle.AspNetCore.Annotations;
//using TrackYourLife.Common.Domain.Core;
//using TrackYourLife.Common.Domain.Core.Shared;
//using TrackYourLife.Common.Presentation.Abstractions;
//using TrackYourLife.Modules.Users.Application.Core.Abstraction;
//using TrackYourLife.Modules.Users.Application.Goals.Commands.AddGoal;
//using TrackYourLife.Modules.Users.Application.Goals.Commands.UpdateGoal;
//using TrackYourLife.Modules.Users.Application.Goals.Queries.GetActiveGoalByType;
//using TrackYourLife.Modules.Users.Contracts.Goals;
//using TrackYourLife.Modules.Users.Domain.Goals;
//using TrackYourLife.Modules.Users.Presentation.Contracts;

//namespace TrackYourLife.Modules.Users.Presentation.Controllers;

//public class GoalController(ISender sender, IUsersMapper mapper, IFeatureManager featureManager)
//    : ApiController(featureManager)
//{
//    [HttpGet(ApiRoutes.UserGoal.GetActiveGoal)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.GetActiveGoal))]
//    [ProducesResponseType(typeof(GoalDto), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> GetActiveUserGoalByTypeAsync(
//        [FromQuery, Required] GoalType GoalType,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(
//                new GetActiveGoalByTypeQuery(GoalType),
//                DomainErrors.General.UnProcessableRequest
//            )
//            .BindAsync(query => sender.Send(query, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [HttpPost(ApiRoutes.UserGoal.AddGoal)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.AddGoal))]
//    [ProducesResponseType(typeof(GoalDto), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> AddUserGoalAsync(
//        AddUserGoalRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(request, DomainErrors.General.UnProcessableRequest)
//            .Map(mapper.Map<AddGoalCommand>)
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [HttpPut(ApiRoutes.UserGoal.UpdateGoal)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.UserGoal.UpdateGoal))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> UpdateUserGoalAsync(
//        UpdateUserGoalRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(request, DomainErrors.General.UnProcessableRequest)
//            .Map(mapper.Map<UpdateUserGoalCommand>)
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }
//}
