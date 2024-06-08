using Microsoft.EntityFrameworkCore;
using TrackYourLife.Domain.Users;
using TrackYourLife.Domain.Users.Repositories;
using TrackYourLife.Domain.Users.StrongTypes;
using TrackYourLife.Persistence.Specifications;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Repositories;

internal sealed class UserTokenRepository
    : GenericRepository<UserToken, UserTokenId>,
        IUserTokenRepository
{
    public UserTokenRepository(ApplicationWriteDbContext context)
        : base(context.UserTokens) { }

    public async Task<UserToken?> GetByValueAsync(
        string value,
        CancellationToken cancellationToken
    ) => await FirstOrDefaultAsync(new UserTokenWithValueSpecification(value), cancellationToken);

    public async Task<UserToken?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new UserTokenWithUserIdSpecification(userId),
            cancellationToken
        );
    }
}
