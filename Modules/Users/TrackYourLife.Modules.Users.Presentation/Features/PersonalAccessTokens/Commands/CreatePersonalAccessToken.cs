using TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.Modules.Users.Presentation.Features.PersonalAccessTokens.Commands;

internal sealed record CreatePersonalAccessTokenRequest(string Name);

internal sealed class CreatePersonalAccessToken(ISender sender)
    : Endpoint<CreatePersonalAccessTokenRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<PersonalAccessTokensGroup>();
        Description(x =>
            x.Produces<CreatedPersonalAccessTokenDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreatePersonalAccessTokenRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(r => new CreatePersonalAccessTokenCommand(r.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(dto => dto);
    }
}
