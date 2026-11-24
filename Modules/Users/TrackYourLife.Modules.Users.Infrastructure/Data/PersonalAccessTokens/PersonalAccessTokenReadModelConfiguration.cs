using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.PersonalAccessTokens;

internal sealed class PersonalAccessTokenReadModelConfiguration
    : IEntityTypeConfiguration<PersonalAccessTokenReadModel>
{
    public void Configure(EntityTypeBuilder<PersonalAccessTokenReadModel> builder)
    {
        builder.ToTable(TableNames.PersonalAccessTokens);

        builder.HasKey(token => token.Id);

        builder.Property(token => token.Name).HasMaxLength(100).IsRequired();

        builder.Property(token => token.TokenPrefix).HasMaxLength(32).IsRequired();

        builder.HasOne<UserReadModel>().WithMany().HasForeignKey(token => token.UserId).IsRequired();
    }
}
