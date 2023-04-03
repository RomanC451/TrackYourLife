using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Persistence.Constants;

namespace Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable(TableNames.RefreshTokens);

        builder.HasKey(token => token.Id);
        builder.Property(token => token.UserId);
        builder.HasIndex(token => token.UserId).IsUnique();
        builder.Property(token => token.Value);
        builder.HasIndex(token => token.Value).IsUnique();
        builder.Property(token => token.CreatedOn);
        builder.Property(token => token.ExpiresAt);
    }
}
