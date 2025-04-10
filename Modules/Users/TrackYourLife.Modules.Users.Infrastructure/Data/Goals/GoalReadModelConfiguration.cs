using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals;

internal sealed class GoalReadModelConfiguration : IEntityTypeConfiguration<GoalReadModel>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<GoalReadModel> builder
    )
    {
        builder.ToTable(TableNames.Goals);
        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.Type).HasConversion<string>();

        builder.Property(goal => goal.Period).HasConversion<string>();

        builder.HasOne<UserReadModel>().WithMany().HasForeignKey(g => g.UserId).IsRequired();
    }
}
