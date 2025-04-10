using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Outbox;

internal sealed class NutritionOutboxMessageRepository(NutritionWriteDbContext dbContext)
    : OutboxMessageRepository(dbContext.OutboxMessages),
        INutritionOutboxMessageRepository;
