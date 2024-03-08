using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Users.Repositories;

public interface IUserTokenRepository
{
    Task<UserToken?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<UserToken?> GetByUserIdAsync(UserId id, CancellationToken cancellationToken);
    Task AddAsync(UserToken token, CancellationToken cancellationToken);

    void Remove(UserToken token);

    void Update(UserToken token);
}
