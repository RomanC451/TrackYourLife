using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.Books;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable(TableNames.Books);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Author).HasMaxLength(300).IsRequired();
        builder.Property(x => x.TotalPages).IsRequired();
        builder.Property(x => x.CurrentPage).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);
    }
}
