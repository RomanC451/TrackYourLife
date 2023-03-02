using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
    void Remove(User user);
    void Update(User user);
}
