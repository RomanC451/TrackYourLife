using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.Application.Users.Queries.GetUserById;
using TrackYourLife.Domain.Shared;
using TrackYourLife.Application.Users.Commands.Remove;
using TrackYourLife.Application.Users.Commands.VerifyEmail;
using TrackYourLife.Application.Users.Commands.ResendVerificationEmail;
using MapsterMapper;
using Microsoft.FeatureManagement;
using TrackYourLife.Domain.Errors;
using TrackYourLife.Contracts.Users;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Application.Users.Commands.Update;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using TrackYourLife.Application.Users.Commands.UploadUserProfileImage;
using System.ComponentModel.DataAnnotations;
using TrackYourLife.Contracts.Common;
using TrackYourLife.Common.Presentation.Contracts;
using TrackYourLife.Common.Presentation.Abstractions;

namespace TrackYourLife.Common.Presentation.Controllers;

public sealed class UserController(ISender sender, IMapper mapper, IFeatureManager featureManager) : ApiController(sender, mapper, featureManager)
{
    [HttpDelete(ApiRoutes.User.Delete)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.Delete))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(CancellationToken cancellationToken)
    {
        return await Result
            .Create(new RemoveUserCommand())
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [HttpPut(ApiRoutes.User.Update)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.Update))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<UpdateUserCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.User.VerifyEmail)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.VerifyEmail))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyEmailAsync(
        string token,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(new VerifyEmailCommand(token), DomainErrors.General.UnProcessableRequest)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.User.ResendVerificationEmail)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.ResendVerificationEmail))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResendVerificationEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(
                new ResendEmailVerificationCommand(email),
                DomainErrors.General.UnProcessableRequest
            )
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [HttpGet(ApiRoutes.User.GetCurrentUserData)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.GetCurrentUserData))]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUserDataAsync(CancellationToken cancellationToken)
    {
        return await Result
            .Create(new GetUserDataQuery())
            .Bind(query => _sender.Send(query, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [HttpPut(ApiRoutes.User.UploadProfileImage)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.User.UploadProfileImage))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProfileImageAsync(
        [FromForm] FileRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(new UploadUserProfileImageCommand(request.File), DomainErrors.General.UnProcessableRequest)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

}
