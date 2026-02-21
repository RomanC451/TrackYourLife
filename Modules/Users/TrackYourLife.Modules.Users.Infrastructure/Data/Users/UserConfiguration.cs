using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Contracts;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(user => user.Id);

        builder.Property(user => user.ModifiedOnUtc).IsRequired(false);

        builder
            .Property(user => user.Email)
            .HasConversion(email => email.Value, value => Email.Create(value).Value);

        builder.HasIndex(user => user.Email).IsUnique();

        builder
            .Property(user => user.PasswordHash)
            .HasConversion(password => password.Value, value => new HashedPassword(value));

        builder
            .Property(x => x.FirstName)
            .HasConversion(x => x.Value, v => Name.Create(v).Value)
            .HasMaxLength(Name.MaxLength);

        builder
            .Property(x => x.LastName)
            .HasConversion(x => x.Value, v => Name.Create(v).Value)
            .HasMaxLength(Name.MaxLength);

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
