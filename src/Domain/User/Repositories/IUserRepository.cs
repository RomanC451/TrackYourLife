using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;

namespace TrackYourLifeDotnet.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Remove(User user);
    void Update(User user);
}
