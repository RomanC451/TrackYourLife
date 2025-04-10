using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;
using TrackYourLife.Modules.Users.Contracts.Users;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal sealed record LogInUserRequest(string Email, string Password, DeviceId DeviceId);

internal sealed class LoginUser(
    ISender sender,
    IUsersMapper mapper,
    IAuthCookiesManager authCookiesManager
) : Endpoint<LogInUserRequest, IResult>
{
    public override void Configure()
    {
        Post("login");
        Group<AuthenticationGroup>();
        Description(x =>
            x.Produces<TokenResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(LogInUserRequest req, CancellationToken ct)
    {
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<LogInUserCommand>)
            .BindAsync(command => sender.Send(command, ct));

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.ToBadRequestProblemDetails());
        }

        var cookieResult = authCookiesManager.SetRefreshTokenCookie(result.Value.Item2);

        if (cookieResult.IsFailure)
        {
            return TypedResults.BadRequest(error: cookieResult.ToBadRequestProblemDetails());
        }

        return TypedResults.Ok(new TokenResponse(result.Value.Item1));
    }
}
