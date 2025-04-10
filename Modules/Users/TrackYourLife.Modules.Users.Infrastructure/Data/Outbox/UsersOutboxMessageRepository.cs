using TrackYourLife.Modules.Users.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Outbox;

internal sealed class UsersOutboxMessageRepository(UsersWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        IUsersOutboxMessageRepository;
