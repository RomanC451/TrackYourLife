using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;
using TrackYourLife.Modules.Users.Contracts.Users;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users.Queries;

public class GetCurrentUser(ISender sender, IUsersMapper mapper) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("me");
        Group<UsersGroup>();
        Description(x =>
            x.Produces<UserDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var result = await Result
            .Create(new GetCurrentUserQuery())
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.Ok(mapper.Map<UserDto>(result.Value)),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToNoFoundProblemDetails())
        };
    }
}
