using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Persistence.Constants;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Enums;

namespace Persistence.Configurations;

public sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable(TableNames.UserTokens);

        builder.HasKey(token => token.Id);
        builder
            .Property(token => token.Type)
            .HasConversion(
                token => token.ToString(),
                value => (UserTokenTypes)Enum.Parse(typeof(UserTokenTypes), value)
            );
        builder.HasIndex(token => token.Value).IsUnique();

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).IsRequired();
    }
}
