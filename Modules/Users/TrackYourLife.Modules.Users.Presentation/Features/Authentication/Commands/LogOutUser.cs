using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

public class LogOutUser(IAuthCookiesManager authCookiesManager) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("logout");
        Group<AuthenticationGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var result = authCookiesManager.DeleteRefreshTokenCookie();

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.ToBadRequestProblemDetails());
        }

        await Task.CompletedTask;

        return TypedResults.NoContent();
    }
}
