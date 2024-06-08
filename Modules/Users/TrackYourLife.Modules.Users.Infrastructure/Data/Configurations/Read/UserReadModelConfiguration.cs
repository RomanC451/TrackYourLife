using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;


namespace TrackYourLife.Modules.Users.Infrastructure.Data.Configurations.Read;

internal sealed class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToTable(TableNames.Users);
        builder.HasKey(u => u.Id);
    }
}
