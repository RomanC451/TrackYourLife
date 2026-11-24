using TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Presentation.Features.PersonalAccessTokens.Commands;

internal sealed class RevokePersonalAccessToken(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("/{id}");
        Group<PersonalAccessTokensGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<PersonalAccessTokenId>("id")!;

        return await Result
            .Create(new RevokePersonalAccessTokenCommand(id))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
