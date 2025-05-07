using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal sealed record ResendEmailVerificationRequest(string Email);

internal sealed class ResendEmailVerification(ISender sender)
    : Endpoint<ResendEmailVerificationRequest, IResult>
{
    public override void Configure()
    {
        Post("resend-verification-email");
        Group<AuthenticationGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );

        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(
        ResendEmailVerificationRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new ResendEmailVerificationCommand(req.Email))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
