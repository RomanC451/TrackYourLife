using TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.Modules.Users.Presentation.Features.Users.Queries;

internal sealed class GetCurrentUser(ISender sender) : EndpointWithoutRequest<IResult>
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
        return await Result
            .Create(new GetCurrentUserQuery())
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(user => user.ToDto());
    }
}
