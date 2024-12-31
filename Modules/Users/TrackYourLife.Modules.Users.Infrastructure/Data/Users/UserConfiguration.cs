using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.ValueObjects;
using TrackYourLife.Modules.Users.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(TableNames.Users);

        builder.HasKey(user => user.Id);

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
    }
}
