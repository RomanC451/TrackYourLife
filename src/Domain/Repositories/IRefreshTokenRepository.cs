using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByUserIdAsync(Guid id);
    void Add(RefreshToken token);

    void Remove(RefreshToken token);

    void Update(RefreshToken token);
}
