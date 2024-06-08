using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.OutboxMessages;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Write;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(TableNames.OutboxMessages);

        builder.HasKey(x => x.Id);
    }
}
