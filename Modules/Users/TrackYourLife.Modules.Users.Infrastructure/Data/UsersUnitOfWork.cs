using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Users.Infrastructure.Data;

internal sealed class UsersUnitOfWork(UsersWriteDbContext dbContext)
    : UnitOfWork<UsersWriteDbContext>(dbContext),
        IUsersUnitOfWork;
