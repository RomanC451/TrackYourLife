using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Tokens;

internal sealed class TokenReadModelConfiguration : IEntityTypeConfiguration<TokenReadModel>
{
    public void Configure(EntityTypeBuilder<TokenReadModel> builder)
    {
        builder.ToTable(TableNames.Tokens);

        builder.HasKey(token => token.Id);

        builder.Property(t => t.UserId);

        builder.Property(token => token.Type).HasConversion<string>();
    }
}
