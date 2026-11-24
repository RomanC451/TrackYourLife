using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Queries.GetPersonalAccessTokens;

internal sealed class GetPersonalAccessTokensQueryHandler(
    IPersonalAccessTokenQuery personalAccessTokenQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetPersonalAccessTokensQuery, IReadOnlyList<PersonalAccessTokenReadModel>>
{
    public async Task<Result<IReadOnlyList<PersonalAccessTokenReadModel>>> Handle(
        GetPersonalAccessTokensQuery query,
        CancellationToken cancellationToken
    )
    {
        var tokens = await personalAccessTokenQuery.GetActiveByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success(tokens);
    }
}
