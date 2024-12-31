using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;
using TrackYourLife.Modules.Users.Contracts.Users;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

public class RefreshToken(ISender sender, IAuthCookiesManager authCookiesManager)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("refresh-token");
        Group<AuthenticationGroup>();
        Description(x =>
            x.Produces<TokenResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );

        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var refreshTokenValue = authCookiesManager.GetRefreshTokenFromCookie();

        if (refreshTokenValue.IsFailure)
        {
            return TypedResults.Unauthorized();
        }

        var result = await sender.Send(new RefreshJwtTokenCommand(refreshTokenValue.Value), ct);

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.ToBadRequestProblemDetails());
        }

        var cookieResult = authCookiesManager.SetRefreshTokenCookie(result.Value.Item2);

        if (cookieResult.IsFailure)
        {
            return TypedResults.BadRequest(error: cookieResult.ToBadRequestProblemDetails());
        }

        return TypedResults.Ok(result.Value.Item1);
    }
}
