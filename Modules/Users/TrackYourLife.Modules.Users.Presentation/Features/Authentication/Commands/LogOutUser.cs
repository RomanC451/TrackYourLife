using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;
using TrackYourLife.Modules.Users.Domain.Tokens;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal sealed record LogOutUserRequest(DeviceId DeviceId, bool LogOutAllDevices);

internal sealed class LogOutUser(IAuthCookiesManager authCookiesManager, ISender sender)
    : Endpoint<LogOutUserRequest, IResult>
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

    public override async Task<IResult> ExecuteAsync(LogOutUserRequest req, CancellationToken ct)
    {
        var result = await sender.Send(
            new LogOutUserCommand(req.DeviceId, req.LogOutAllDevices),
            ct
        );

        if (result.IsFailure)
        {
            return TypedResults.BadRequest(result.ToBadRequestProblemDetails());
        }

        authCookiesManager.DeleteRefreshTokenCookie();

        await Task.CompletedTask;

        return TypedResults.NoContent();
    }
}
