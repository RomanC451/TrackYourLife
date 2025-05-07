using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;

internal sealed record UpdateCurrentUserRequest(string FirstName, string LastName);

internal sealed class UpdateCurrentUser(ISender sender)
    : Endpoint<UpdateCurrentUserRequest, IResult>
{
    public override void Configure()
    {
        Put("/me");
        Group<UsersGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateCurrentUserRequest req,
        CancellationToken ct
    )
    {
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new UpdateUserCommand(req.FirstName, req.LastName))
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToNoFoundProblemDetails()),
        };
    }
}
