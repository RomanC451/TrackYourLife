using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.ValueObjects;

namespace TrackYourLifeDotnet.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken);
    void Add(User user);
    void Remove(User user);
    void Update(User user);
}
