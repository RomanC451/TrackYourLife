using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

namespace TrackYourLife.Modules.Users.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    void Remove(User user);
    void Update(User user);
}
