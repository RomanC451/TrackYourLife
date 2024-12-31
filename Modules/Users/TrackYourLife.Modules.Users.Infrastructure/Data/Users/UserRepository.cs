using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Data.Users.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal sealed class UserRepository(UsersWriteDbContext context)
    : GenericRepository<User, UserId>(context.Users),
        IUserRepository
{
    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await FirstOrDefaultAsync(new UserWithEmailSpecification(email), cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        return !await AnyAsync(new UserWithEmailSpecification(email), cancellationToken);
    }
}
