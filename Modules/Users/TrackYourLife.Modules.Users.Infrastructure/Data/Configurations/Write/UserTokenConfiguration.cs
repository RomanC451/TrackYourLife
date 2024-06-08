using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;


namespace TrackYourLife.Modules.Users.Infrastructure.Data.Configurations.Write;

public sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable(TableNames.UserTokens);

        builder.HasKey(token => token.Id);
        builder
            .Property(token => token.Id)
            .HasConversion(id => id.Value, value => new UserTokenId(value));

        builder
            .Property(token => token.Type)
            .HasConversion(
                token => token.ToString(),
                value => (UserTokenTypes)Enum.Parse(typeof(UserTokenTypes), value)
            );

        builder.HasIndex(token => token.Value).IsUnique();

        builder
            .Property(token => token.UserId)
            .HasConversion(id => id.Value, value => new UserId(value));

        builder.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).IsRequired();
    }
}
