using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<RefreshToken?> GetByUserId(Guid id);
    void Add(RefreshToken token);

    void Remove(RefreshToken token);

    void Update(RefreshToken token);
}
