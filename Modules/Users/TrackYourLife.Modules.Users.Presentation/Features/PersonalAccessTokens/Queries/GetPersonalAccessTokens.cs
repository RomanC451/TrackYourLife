using TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Queries.GetPersonalAccessTokens;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.Modules.Users.Presentation.Features.PersonalAccessTokens.Queries;

internal sealed class GetPersonalAccessTokens(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<PersonalAccessTokensGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<PersonalAccessTokenDto>>(StatusCodes.Status200OK)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetPersonalAccessTokensQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(tokens => tokens.Select(token => token.ToDto()).ToList());
    }
}
