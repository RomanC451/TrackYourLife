using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.Domain.Shared;
using TrackYourLife.Application.Users.Commands.RefreshJwtToken;
using TrackYourLife.Application.Core.Abstractions.Services;
using MapsterMapper;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Http;
using TrackYourLife.Contracts.Users;
using TrackYourLife.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using TrackYourLife.Application.Users.Commands.LogIn;
using TrackYourLife.Application.Users.Commands.Register;
using Swashbuckle.AspNetCore.Annotations;
using TrackYourLife.Contracts.Common;
using TrackYourLife.Common.Presentation.Contracts;
using TrackYourLife.Common.Presentation.Abstractions;

namespace TrackYourLife.Common.Presentation.Controllers;

public sealed class AuthenticationController(
    ISender sender,
    IAuthService authService,
    IMapper mapper,
    IFeatureManager featureManager
    ) : ApiController(sender, mapper, featureManager)
{
    private readonly IAuthService _authService = authService;

    [AllowAnonymous]
    [HttpPost(ApiRoutes.Authentication.Register)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Authentication.Register))]
    [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<RegisterUserCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.Authentication.LogIn)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Authentication.LogIn))]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync(
        LogInUserRequest request,
        CancellationToken cancellationToken
    )
    {
        return await Result
            .Create(request, DomainErrors.General.UnProcessableRequest)
            .Map(_mapper.Map<LogInUserCommand>)
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }

    [HttpPost(ApiRoutes.Authentication.LogOut)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Authentication.LogOut))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogoutAsync()
    {
        return await _authService.RemoveRefreshTokenCookie().MapAsync(MatchResponse);
    }

    [AllowAnonymous]
    [HttpPost(ApiRoutes.Authentication.RefreshToken)]
    [SwaggerOperation(OperationId = nameof(ApiRoutes.Authentication.RefreshToken))]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokeAsync(CancellationToken cancellationToken)
    {
        return await Result
            .Create(new RefreshJwtTokenCommand())
            .Bind(command => _sender.Send(command, cancellationToken))
            .MapAsync(MatchResponse);
    }
}
