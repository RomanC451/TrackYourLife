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
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Presentation.ControllersResponses.Users;

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

    // private void SetRefreshToken(RefreshToken newRefreshToken)
    // {
    //     CookieOptions cookieOptions =
    //         new()
    //         {
    //             HttpOnly = true,
    //             Secure = false,
    //             Expires = newRefreshToken.ExpiresAt
    //         };

    //     Response.Cookies.Append("refreshToken", newRefreshToken.Value, cookieOptions);
    // }

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

        // SetRefreshToken(result.Value.RefreshToken);

        return Ok(new { userId = result.Value.UserId, jwtToken = result.Value.JwtToken });
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

        // SetRefreshToken(result.Value.RefreshToken);

        var response = new LoginUserControllerResponse(result.Value.UserId, result.Value.JwtToken);

        return Ok(response);
    }

    [Authorize]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveUser()
    {
        string jwtToken = Request.Headers["Authorization"].ToString().Split(" ")[1];

        Guid userId = _authService.GetUserIdFromJwtToken(jwtToken);

        RemoveUserCommand command = new(userId);

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
        string jwtToken = Request.Headers["Authorization"].ToString().Split(" ")[1];

        Guid userId = _authService.GetUserIdFromJwtToken(jwtToken);

        UpdateUserCommand command = new(userId, request.FirstName, request.LastName);

        Result<UpdateUserResponse> response = await Sender.Send(command, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }

        return Ok(response.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        string? refreshToken = Request.Cookies["refreshToken"];

        if (refreshToken is null)
        {
            return BadRequest("Refresh token is missing");
        }

        RefreshJwtTokenCommand command = new(refreshToken);

        Result<RefreshJwtTokenResponse> result = await Sender.Send(command);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        // SetRefreshToken(result.Value.NewRefreshToken);

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
            return NotFound(response.Error);
        }

        return Ok(response.Value);
    }
}
