using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.Features.Users;

public interface IUserQuery
{
    Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken);
    Task<UserReadModel?> GetByIdAsync(UserId userId, CancellationToken cancellationToken);
    Task<UserReadModel?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
}
