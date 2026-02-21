using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Data.Users.Specifications;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal sealed class UserQuery(UsersReadDbContext context)
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

    public async Task<UserReadModel?> GetByStripeCustomerIdAsync(
        string stripeCustomerId,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new UserReadModelWithStripeCustomerIdSpecification(stripeCustomerId),
            cancellationToken
        );
    }
}
