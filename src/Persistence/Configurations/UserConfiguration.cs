using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Persistence.Constants;

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

        builder
            .Property(user => user.Password)
            .HasConversion(password => password.Value, value => PasswordHash.Create(value).Value);

        builder
            .Property(x => x.FirstName)
            .HasConversion(x => x.Value, v => FirstName.Create(v).Value)
            .HasMaxLength(FirstName.MaxLength);

        builder
            .Property(x => x.LastName)
            .HasConversion(x => x.Value, v => LastName.Create(v).Value)
            .HasMaxLength(LastName.MaxLength);

        builder.HasIndex(x => x.Email).IsUnique();

        builder
            .HasOne(x => x.RefreshToken)
            .WithOne(x => x.User)
            .HasForeignKey<RefreshToken>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
