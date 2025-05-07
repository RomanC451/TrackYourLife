using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal record VerifyEmailRequest(string Token);

internal sealed class VerifyEmail(ISender sender) : Endpoint<VerifyEmailRequest, IResult>
{
    public override void Configure()
    {
        Post("verify-email");
        Group<AuthenticationGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(VerifyEmailRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new VerifyEmailCommand(req.Token))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
