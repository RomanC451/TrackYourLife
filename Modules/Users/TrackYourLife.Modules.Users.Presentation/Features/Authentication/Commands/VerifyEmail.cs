using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

namespace TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;

public class VerifyEmail(ISender sender) : EndpointWithoutRequest<IResult>
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

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        string token = Query<string>("token")!;

        var result = await Result
            .Create(new VerifyEmailCommand(token))
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToNoFoundProblemDetails())
        };
    }
}
