using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.Users.Queries;

public interface IUserQuery
{
    Task<bool> UserExistsAsync(UserId userId, CancellationToken cancellationToken);
}
