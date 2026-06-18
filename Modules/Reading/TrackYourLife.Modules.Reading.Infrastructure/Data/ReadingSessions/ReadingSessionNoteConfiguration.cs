using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.Modules.Reading.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.ReadingSessions;

internal sealed class ReadingSessionNoteConfiguration : IEntityTypeConfiguration<ReadingSessionNote>
{
    public void Configure(EntityTypeBuilder<ReadingSessionNote> builder)
    {
        builder.ToTable(TableNames.ReadingSessionNotes);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ReadingSessionId).IsRequired();
        builder.Property(x => x.BookId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.ChapterTitle).IsRequired().HasMaxLength(ReadingSessionNote.MaxChapterTitleLength);
        builder.Property(x => x.Content).IsRequired().HasMaxLength(ReadingSessionNote.MaxContentLength);
        builder.Property(x => x.CreatedOnUtc).IsRequired();

        builder
            .HasOne<ReadingSession>()
            .WithMany()
            .HasForeignKey(x => x.ReadingSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
