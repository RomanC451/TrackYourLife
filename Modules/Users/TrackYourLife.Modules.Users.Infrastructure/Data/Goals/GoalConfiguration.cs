using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Goals;

public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Goal> builder
    )
    {
        builder.ToTable(TableNames.Goals);
        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.Type).HasConversion<string>();

        builder.Property(goal => goal.Period).HasConversion<string>();


        builder.HasOne<User>().WithMany().HasForeignKey(g => g.UserId).IsRequired();
    }
}
