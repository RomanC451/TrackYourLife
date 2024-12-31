//using MapsterMapper;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.FeatureManagement;
//using Swashbuckle.AspNetCore.Annotations;
//using TrackYourLife.Common.Contracts.Common;
//using TrackYourLife.Common.Domain.Core;
//using TrackYourLife.Common.Domain.Core.Shared;
//using TrackYourLife.Common.Presentation.Abstractions;
//using TrackYourLife.Modules.Users.Application.Core.Abstraction;
//using TrackYourLife.Modules.Users.Application.Users.Commands.Remove;
//using TrackYourLife.Modules.Users.Application.Users.Commands.ResendVerificationEmail;
//using TrackYourLife.Modules.Users.Application.Users.Commands.Update;
//using TrackYourLife.Modules.Users.Application.Users.Commands.UploadUserProfileImage;
//using TrackYourLife.Modules.Users.Application.Users.Commands.VerifyEmail;
//using TrackYourLife.Modules.Users.Application.Users.Queries.GetUserById;
//using TrackYourLife.Modules.Users.Contracts.Users;
//using TrackYourLife.Modules.Users.Presentation.Contracts;

//namespace TrackYourLife.Modules.Users.Presentation.Controllers;

//public sealed class UserController(
//    ISender sender,
//    IUsersMapper mapper,
//    IFeatureManager featureManager
//) : ApiController(featureManager)
//{
//    [HttpDelete(ApiRoutes.User.DeleteCurrentUser)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.DeleteCurrentUser))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DeleteAsync(CancellationToken cancellationToken)
//    {
//        return await Result
//            .Create(new RemoveUserCommand())
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [HttpPut(ApiRoutes.User.UpdateCurrentUser)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.UpdateCurrentUser))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> UpdateAsync(
//        UpdateUserRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(request, DomainErrors.General.UnProcessableRequest)
//            .Map(mapper.Map<UpdateUserCommand>)
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [AllowAnonymous]
//    [HttpPost(ApiRoutes.User.VerifyEmail)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.VerifyEmail))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> VerifyEmailAsync(
//        string token,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(new VerifyEmailCommand(token), DomainErrors.General.UnProcessableRequest)
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [AllowAnonymous]
//    [HttpPost(ApiRoutes.User.ResendVerificationEmail)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.ResendVerificationEmail))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> ResendVerificationEmailAsync(
//        string email,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(
//                new ResendEmailVerificationCommand(email),
//                DomainErrors.General.UnProcessableRequest
//            )
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [HttpGet(ApiRoutes.User.GetCurrentUser)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.GetCurrentUser))]
//    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> GetCurrentUserDataAsync(CancellationToken cancellationToken)
//    {
//        return await Result
//            .Create(new GetUserDataQuery())
//            .BindAsync(query => sender.Send(query, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [HttpPut(ApiRoutes.User.UploadProfileImage)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.UploadProfileImage))]
//    [ProducesResponseType(StatusCodes.Status204NoContent)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> UploadProfileImageAsync(
//        [FromForm] FileRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(
//                new UploadUserProfileImageCommand(request.File),
//                DomainErrors.General.UnProcessableRequest
//            )
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }
//}
