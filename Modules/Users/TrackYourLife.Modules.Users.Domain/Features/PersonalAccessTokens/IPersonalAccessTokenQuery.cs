using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

public interface IPersonalAccessTokenQuery
{
    Task<IReadOnlyList<PersonalAccessTokenReadModel>> GetActiveByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    );

    Task<PersonalAccessTokenReadModel?> GetByIdAsync(
        PersonalAccessTokenId id,
        CancellationToken cancellationToken
    );
}
