using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Common.Persistence.Constants;
using TrackYourLife.Common.Domain.OutboxMessages;

namespace TrackYourLife.Common.Persistence.Configurations.Write;

internal sealed class OutboxMessageConsumerConfiguration
    : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable(TableNames.OutboxMessageConsumers);

        builder.HasKey(
            outboxMessageConsumer => new { outboxMessageConsumer.Id, outboxMessageConsumer.Name }
        );
    }
}
