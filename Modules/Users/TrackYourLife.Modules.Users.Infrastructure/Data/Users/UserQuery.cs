using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Data.Users.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal class UserQuery(UsersReadDbContext context)
    : GenericQuery<UserReadModel, UserId>(context.Users),
        IUserQuery
{
    public async Task<UserReadModel?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new UserReadModelWithEmailSpecification(email),
            cancellationToken
        );
    }
}
