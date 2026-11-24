using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.PersonalAccessTokens;

internal sealed class PersonalAccessTokenConfiguration
    : IEntityTypeConfiguration<PersonalAccessToken>
{
    public void Configure(EntityTypeBuilder<PersonalAccessToken> builder)
    {
        builder.ToTable(TableNames.PersonalAccessTokens);

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Name).HasMaxLength(100).IsRequired();

        builder.Property(token => token.TokenPrefix).HasMaxLength(32).IsRequired();

        builder.Property(token => token.TokenHash).HasMaxLength(128).IsRequired();

        builder.HasIndex(token => token.TokenHash).IsUnique();

        builder.HasIndex(token => token.UserId);

        builder.HasOne<User>().WithMany().HasForeignKey(token => token.UserId).IsRequired();

        builder
            .Property(token => token.CreatedOnUtc)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(token => token.LastUsedOnUtc).HasColumnType("timestamp with time zone");

        builder
            .Property(token => token.ExpiresAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(token => token.RevokedOnUtc).HasColumnType("timestamp with time zone");

        builder.Ignore(token => token.IsRevoked);
    }
}
