using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;


namespace TrackYourLife.Modules.Users.Infrastructure.Data.Configurations.Write;

public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserGoal> builder
    )
    {
        builder.ToTable(TableNames.UserGoals);
        builder.HasKey(goal => goal.Id);

        builder
            .Property(goal => goal.Id)
            .HasConversion(id => id.Value, value => new UserGoalId(value));

        builder
            .Property(goal => goal.Type)
            .HasConversion(
                goal => goal.ToString(),
                value => (UserGoalType)Enum.Parse(typeof(UserGoalType), value)
            );

        builder
            .Property(goal => goal.PerPeriod)
            .HasConversion(
                goal => goal.ToString(),
                value => (UserGoalPerPeriod)Enum.Parse(typeof(UserGoalPerPeriod), value)
            );

        builder
            .Property(goal => goal.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.HasOne<User>().WithMany().HasForeignKey(g => g.UserId).IsRequired();
    }
}
