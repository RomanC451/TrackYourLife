using Microsoft.EntityFrameworkCore;
using TrackYourLife.Domain.Users;
using TrackYourLife.Domain.Users.Repositories;
using TrackYourLife.Domain.Users.StrongTypes;
using TrackYourLife.Domain.Users.ValueObjects;
using TrackYourLife.Persistence.Specifications;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Repositories;

internal sealed class UserRepository : GenericRepository<User, UserId>, IUserRepository
{
    public UserRepository(ApplicationWriteDbContext context)
        : base(context.Users) { }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
    {
        return await FirstOrDefaultAsync(new UserWithEmailSpecification(email), cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, CancellationToken cancellationToken)
    {
        return !await AnyAsync(new UserWithEmailSpecification(email), cancellationToken);
    }
}
