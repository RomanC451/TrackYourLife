using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.Modules.Reading.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionReadModelConfiguration
    : IEntityTypeConfiguration<ReadingSessionReadModel>
{
    public void Configure(EntityTypeBuilder<ReadingSessionReadModel> builder)
    {
        builder.ToTable(TableNames.ReadingSessions);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.BookId).IsRequired();
        builder.Property(x => x.StartPage).IsRequired();
        builder.Property(x => x.StartedOnUtc).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Ignore(x => x.IsActive);
        builder.Ignore(x => x.BookTitle);
        builder.Ignore(x => x.BookAuthor);
    }
}
