using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Users.Queries;

public interface IUserQuery
{
    Task<bool> UserExistsAsync(UserId userId, CancellationToken cancellationToken);
}
