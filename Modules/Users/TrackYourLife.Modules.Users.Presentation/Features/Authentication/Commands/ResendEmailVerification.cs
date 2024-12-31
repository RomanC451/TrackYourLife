using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

internal sealed record ResendEmailVerificationRequest
{
    [QueryParam]
    public string Email { get; init; } = string.Empty;
}

internal class ResendEmailVerification(ISender sender)
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
    }

    public override async Task<IResult> ExecuteAsync(
        ResendEmailVerificationRequest req,
        CancellationToken ct
    )
    {
        var result = await Result
            .Create(new ResendEmailVerificationCommand(req.Email))
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToNoFoundProblemDetails())
        };
    }
}
