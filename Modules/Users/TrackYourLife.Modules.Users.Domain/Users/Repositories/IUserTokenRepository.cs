using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.Users.Repositories;

public interface IUserTokenRepository
{
    Task<UserToken?> GetByValueAsync(string value, CancellationToken cancellationToken);

    Task<UserToken?> GetByUserIdAsync(UserId id, CancellationToken cancellationToken);
    Task AddAsync(UserToken token, CancellationToken cancellationToken);

    void Remove(UserToken token);

    void Update(UserToken token);
}
