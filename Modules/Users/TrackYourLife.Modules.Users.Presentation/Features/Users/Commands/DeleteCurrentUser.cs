using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;

public class DeleteUser(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("me");
        Group<UsersGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var result = await Result
            .Create(new RemoveCurrentUserCommand())
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            _ => TypedResults.NotFound(result.ToNoFoundProblemDetails())
        };
    }
}