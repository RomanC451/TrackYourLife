using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Outbox;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(TableNames.OutboxMessages);

        builder.HasKey(x => x.Id);
    }
}
