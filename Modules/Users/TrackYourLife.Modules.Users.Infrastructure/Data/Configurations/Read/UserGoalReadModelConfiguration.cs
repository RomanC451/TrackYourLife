using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;


namespace TrackYourLife.Modules.Users.Infrastructure.Data.Configurations.Read;

internal sealed class UserGoalReadModelConfiguration : IEntityTypeConfiguration<UserGoalReadModel>
{
    public void Configure(EntityTypeBuilder<UserGoalReadModel> builder)
    {
        builder.ToTable(TableNames.UserGoals);
        builder.HasKey(g => g.Id);

        builder.HasOne<UserReadModel>().WithMany().HasForeignKey(g => g.UserId);
    }
}
