using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Application.Users.Commands.Register;
using TrackYourLifeDotnet.Application.Users.Commands.Update;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Application.Users.Queries;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserById;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Abstractions;
using TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Presentation.ControllersResponses.Users;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;

namespace TrackYourLifeDotnet.Presentation.Controllers;

[Route("api/[controller]")]
public sealed class UsersController : ApiController
{
    private readonly IAuthService _authService;

    public UsersController(ISender sender, IAuthService authService)
        : base(sender)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        RegisterUserCommand command =
            new(request.Email, request.Password, request.FirstName, request.LastName);

        Result<RegisterUserResponse> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        RegisterUserControllerResponse response = new(result.Value.UserId, result.Value.JwtToken);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken
    )
    {
        LoginUserCommand command = new(request.Email, request.Password);

        Result<LoginUserResponse> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        var response = new LoginUserControllerResponse(result.Value.UserId, result.Value.JwtToken);

        return Ok(response);
    }

    [Authorize]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveUser()
    {
        var jwtTokenResult = _authService.GetHttpContextJwtToken();

        if (jwtTokenResult.IsFailure)
        {
            return HandleFailure(jwtTokenResult);
        }

        var command = new RemoveUserCommand(jwtTokenResult.Value);

        Result<RemoveUserResponse> result = await Sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(
        UpdateUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var jwtTokenResult = _authService.GetHttpContextJwtToken();

        if (jwtTokenResult.IsFailure)
        {
            return HandleFailure(jwtTokenResult);
        }

        UpdateUserCommand command = new(jwtTokenResult.Value, request.FirstName, request.LastName);

        Result<UpdateUserResponse> response = await Sender.Send(command, cancellationToken);

        if (response.IsFailure)
        {
            return HandleFailure(response);
        }

        return Ok(response.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        var refreshToken = _authService.GetRefreshTokenFromCookie();

        if (refreshToken.IsFailure)
        {
            return HandleFailure(refreshToken);
        }

        RefreshJwtTokenCommand command = new(refreshToken.Value);

        Result<RefreshJwtTokenResponse> result = await Sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value.NewJwtToken);
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        GetUserByIdQuery query = new(id);

        Result<GetUserResponse> response = await Sender.Send(query, cancellationToken);

        if (response.IsFailure)
        {
            return HandleFailure(response);
        }

        return Ok(response.Value);
    }
}
