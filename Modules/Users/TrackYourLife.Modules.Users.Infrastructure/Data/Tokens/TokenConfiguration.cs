using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens;

internal sealed class TokenConfiguration : IEntityTypeConfiguration<Token>
{
    public void Configure(EntityTypeBuilder<Token> builder)
    {
        builder.ToTable(TableNames.Tokens);

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Type).HasConversion<string>();

        builder.HasIndex(token => token.Value).IsUnique();

        builder.HasIndex(token => token.DeviceId);

        builder.HasOne<User>().WithMany().HasForeignKey(token => token.UserId).IsRequired();

        builder
            .Property(token => token.CreatedOn)
            .IsRequired()
            .HasColumnType("timestamp with time zone");
    }
}
