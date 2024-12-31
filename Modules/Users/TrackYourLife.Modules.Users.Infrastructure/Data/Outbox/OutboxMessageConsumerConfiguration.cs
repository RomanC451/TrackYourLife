using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.OutboxMessages;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Outbox;

internal sealed class OutboxMessageConsumerConfiguration
    : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable(TableNames.OutboxMessageConsumers);

        builder.HasKey(outboxMessageConsumer => new
        {
            outboxMessageConsumer.Id,
            outboxMessageConsumer.Name
        });
    }
}
