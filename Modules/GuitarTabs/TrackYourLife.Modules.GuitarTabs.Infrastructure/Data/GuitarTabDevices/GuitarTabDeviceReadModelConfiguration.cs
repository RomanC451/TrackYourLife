using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarTabDevices;

internal sealed class GuitarTabDeviceReadModelConfiguration
    : IEntityTypeConfiguration<GuitarTabDeviceReadModel>
{
    public void Configure(EntityTypeBuilder<GuitarTabDeviceReadModel> builder)
    {
        builder.ToTable(TableNames.GuitarTabDevice);

        builder.HasKey(device => device.Id);

        builder
            .Property(device => device.Id)
            .HasColumnName("DeviceId")
            .HasConversion(id => id.Value, value => GuitarTabDeviceId.Create(value));

        builder.Property(device => device.UserId).IsRequired();

        builder.Property(device => device.Name).IsRequired().HasMaxLength(GuitarTabDevice.NameMaxLength);

        builder.Property(device => device.Speed).HasPrecision(4, 1).IsRequired();

        builder.Property(device => device.Semitones).IsRequired();

        builder.Property(device => device.UseFlats).IsRequired();

        builder.Property(device => device.CreatedOnUtc).IsRequired();

        builder.Property(device => device.ModifiedOnUtc);
    }
}
