using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IUserTokenRepository
{
    Task<UserToken?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<UserToken?> GetByUserIdAsync(Guid id);
    void Add(UserToken token);

    void Remove(UserToken token);

    void Update(UserToken token);
}
