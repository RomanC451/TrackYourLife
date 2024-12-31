//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using TrackYourLife.Modules.Users.Application.Core.Abstraction;
//using TrackYourLife.Modules.Users.Application.Users.Commands.LogIn;
//using TrackYourLife.Modules.Users.Application.Users.Commands.RefreshJwtToken;
//using TrackYourLife.Modules.Users.Application.Users.Commands.Register;
//using TrackYourLife.Modules.Users.Contracts.Users;
//using TrackYourLife.Modules.Users.Domain.Tokens;
//using TrackYourLife.Modules.Users.Presentation.Contracts;

//namespace TrackYourLife.Modules.Users.Presentation.Controllers;

//public sealed class AuthenticationController(
//    IHttpContextAccessor httpContextAccessor,
//    ISender sender,
//    IUsersMapper mapper
//) : ApiController(featureManager)
//{
//    private readonly HttpContext? httpContext = httpContextAccessor.HttpContext;

//    readonly CookieOptions cookieOptions =
//        new()
//        {
//            Expires = DateTime.UtcNow.AddDays(7),
//            HttpOnly = true,
//            IsEssential = true,
//            Secure = true,
//            SameSite = SameSiteMode.None,
//            //Domain = "192.168.1.6",
//        };

//    [AllowAnonymous]
//    [HttpPost(ApiRoutes.Authentication.Register)]
//    [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> RegisterAsync(
//        RegisterUserRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        return await Result
//            .Create(request, DomainErrors.General.UnProcessableRequest)
//            .Map(mapper.Map<RegisterUserCommand>)
//            .BindAsync(command => sender.Send(command, cancellationToken))
//            .MapAsync(MatchResponseAsync);
//    }

//    [AllowAnonymous]
//    [HttpPost(ApiRoutes.Authentication.LogIn)]
//    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> LoginAsync(
//        LogInUserRequest request,
//        CancellationToken cancellationToken
//    )
//    {
//        var result = await Result
//            .Create(request, DomainErrors.General.UnProcessableRequest)
//            .Map(mapper.Map<LogInUserCommand>)
//            .BindAsync(command => sender.Send(command, cancellationToken));

//        if (result.IsFailure)
//        {
//            return await result.MapAsync(MatchResponseAsync);
//        }

//        SetRefreshTokenCookie(result.Value.Item2);

//        return Ok(result.Value.Item1);
//    }

//    [HttpPost(ApiRoutes.Authentication.LogOut)]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> LogoutAsync()
//    {
//        if (httpContext is null)
//        {
//            return await Result
//                .Failure(InfrastructureErrors.HttpContext.NotExists)
//                .MapAsync(MatchResponseAsync);
//        }

//        cookieOptions.Expires = DateTime.UtcNow;

//        httpContext.Response.Cookies.Delete("refreshToken", cookieOptions);

//        return Ok();
//    }

//    [AllowAnonymous]
//    [HttpPost(ApiRoutes.Authentication.RefreshToken)]
//    [SwaggerOperation(OperationId = nameof(ApiRoutes.Authentication.RefreshToken))]
//    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> RefreshTokeAsync(CancellationToken cancellationToken)
//    {
//        var result = await Result
//            .Create(new RefreshJwtTokenCommand())
//            .BindAsync(command => sender.Send(command, cancellationToken));

//        if (result.IsFailure)
//        {
//            return await result.MapAsync(MatchResponseAsync);
//        }

//        SetRefreshTokenCookie(result.Value.Item2);

//        return Ok(result.Value);
//    }

//    private Result SetRefreshTokenCookie(Token refreshToken)
//    {
//        cookieOptions.Expires = refreshToken.ExpiresAt;

//        if (string.IsNullOrEmpty(refreshToken.Value))
//        {
//            return Result.Failure(TokenErrors.RefreshToken.Invalid);
//        }

//        if (httpContext is null)
//        {
//            return Result.Failure(InfrastructureErrors.HttpContext.NotExists);
//        }

//        httpContext.Response.Cookies.Append("refreshToken", refreshToken.Value, cookieOptions);

//        return Result.Success();
//    }
//}
