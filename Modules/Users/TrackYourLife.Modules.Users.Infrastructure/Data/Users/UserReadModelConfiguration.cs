using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Contracts;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal sealed class UserReadModelConfiguration : IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(user => user.Id);

        builder.HasIndex(user => user.Email).IsUnique();

        builder.Property(user => user.PlanType).HasConversion<string>().HasMaxLength(16);
        builder.Property(user => user.StripeCustomerId).HasMaxLength(256).IsRequired(false);
        builder.Property(user => user.SubscriptionEndsAtUtc).IsRequired(false);
        builder
            .Property(user => user.SubscriptionStatus)
            .HasConversion(
                v => v == null ? null : v.Value.ToStripeString(),
                v => SubscriptionStatusMapping.Parse(v))
            .HasMaxLength(32)
            .IsRequired(false);
        builder.Property(user => user.SubscriptionCancelAtPeriodEnd);
    }
}
