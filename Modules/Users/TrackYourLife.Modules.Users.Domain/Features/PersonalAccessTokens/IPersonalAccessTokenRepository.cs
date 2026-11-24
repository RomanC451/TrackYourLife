using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

public interface IPersonalAccessTokenRepository
{
    Task<PersonalAccessToken?> GetByIdAsync(
        PersonalAccessTokenId id,
        CancellationToken cancellationToken
    );

    Task<PersonalAccessToken?> GetActiveByTokenHashAsync(
        string tokenHash,
        DateTime utcNow,
        CancellationToken cancellationToken
    );

    Task AddAsync(PersonalAccessToken token, CancellationToken cancellationToken);

    void Update(PersonalAccessToken token);
}
