using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Application.Users.Commands.Register;
using TrackYourLifeDotnet.Application.Users.Commands.Update;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Application.Users.Queries;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserData;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Abstractions;
using TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.VerifyEmail;
using TrackYourLifeDotnet.Application.Users.Commands.ResendVerificationEmail;
using MapsterMapper;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Presentation.Controllers;

[Route("api/[controller]")]
public sealed class UserController : ApiController
{
    private readonly IAuthService _authService;

    public UserController(ISender sender, IAuthService authService, IMapper mapper)
        : base(sender, mapper)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = _mapper.Map<RegisterUserCommand>(request);

        Result<RegisterUserResult> result = await _sender.Send(command, cancellationToken);

        return MatchResponse<RegisterUserResult, RegisterUserResponse>(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = _mapper.Map<LoginUserCommand>(request);

        Result<LoginUserResult> result = await _sender.Send(command, cancellationToken);

        return MatchResponse<LoginUserResult, LoginUserResponse>(result);
    }

    [HttpPost("logout")]
    public IActionResult LogoutUser(CancellationToken cancellationToken)
    {
        Result<string> jwtTokenResult = _authService.GetHttpContextJwtToken();

        if (jwtTokenResult.IsFailure)
        {
            return HandleFailure(jwtTokenResult);
        }

        Result result = _authService.RemoveRefreshTokenCookie();

        return MatchResponse(result);
    }

    [Authorize]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveUser()
    {
        var command = new RemoveUserCommand();

        Result result = await _sender.Send(command);

        return MatchResponse(result);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        Result<string> jwtTokenResult = _authService.GetHttpContextJwtToken();

        if (jwtTokenResult.IsFailure)
        {
            return HandleFailure(jwtTokenResult);
        }

        UpdateUserCommand command =
            new(jwtTokenResult.Value, request.FirstName!, request.LastName!);

        Result<UpdateUserResult> response = await _sender.Send(command, cancellationToken);

        return MatchResponse<UpdateUserResult, UpdateUserResponse>(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshTokenResult = _authService.GetRefreshTokenFromCookie();

        if (refreshTokenResult.IsFailure)
        {
            return HandleFailure(refreshTokenResult);
        }

        RefreshJwtTokenCommand command = new(refreshTokenResult.Value);

        Result<RefreshJwtTokenResult> result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        var response = _mapper.Map<RefreshJwtTokenResult>(result.Value);

        return Ok(response.NewJwtTokenString);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        [FromQuery] VerifyEmailRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = _mapper.Map<VerifyEmailCommand>(request);

        Result<VerifyEmailResult> result = await _sender.Send(command, cancellationToken);

        return MatchResponse<VerifyEmailResult, VerifyEmailResponse>(result);
    }

    [HttpPost("resend-verification-email")]
    public async Task<IActionResult> ResendVerificationEmail(
        [FromBody] ResendEmailVerificationRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = _mapper.Map<ResendEmailVerificationCommand>(request);

        Result result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok("Verification email has been sent.");
    }

    [Authorize]
    [HttpGet()]
    public async Task<IActionResult> GetUserData(CancellationToken cancellationToken)
    {
        var query = new GetUserDataQuery();

        Result<GetUserDataResult> response = await _sender.Send(query, cancellationToken);

        return MatchResponse<GetUserDataResult, GetUserDataResponse>(response);
    }
}
