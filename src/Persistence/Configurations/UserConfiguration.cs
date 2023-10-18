using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Persistence.Constants;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Persistence.Configurations;

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
            .Property(user => user.Password)
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
