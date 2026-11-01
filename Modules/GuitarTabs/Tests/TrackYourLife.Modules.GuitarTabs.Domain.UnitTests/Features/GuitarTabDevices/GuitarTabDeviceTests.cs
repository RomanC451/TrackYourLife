using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.UnitTests.Features.GuitarTabDevices;

public sealed class GuitarTabDeviceTests
{
    private static readonly UserId UserId = UserId.NewId();
    private static readonly DateTime CreatedOn = new(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithTheRefreshTokenDeviceId_StoresThatIdWithTheSettings()
    {
        var deviceId = GuitarTabDeviceId.NewId();

        var result = GuitarTabDevice.Create(
            deviceId,
            UserId,
            "  Laptop  ",
            0.2m,
            0,
            false,
            CreatedOn
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(deviceId);
        result.Value.Name.Should().Be("Laptop");
        result.Value.Speed.Should().Be(0.2m);
        result.Value.Semitones.Should().Be(0);
        result.Value.UseFlats.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_Fails(string name)
    {
        var result = GuitarTabDevice.Create(
            GuitarTabDeviceId.NewId(),
            UserId,
            name,
            0.2m,
            0,
            false,
            CreatedOn
        );

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateSettings_ReplacesSpeedAndTranspose()
    {
        var device = GuitarTabDevice
            .Create(GuitarTabDeviceId.NewId(), UserId, "Phone", 0.2m, 0, false, CreatedOn)
            .Value;

        var result = device.UpdateSettings(1.5m, -2, true, CreatedOn.AddMinutes(5));

        result.IsSuccess.Should().BeTrue();
        device.Speed.Should().Be(1.5m);
        device.Semitones.Should().Be(-2);
        device.UseFlats.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(20.1)]
    [InlineData(0.25)]
    public void UpdateSettings_WithSpeedOutsideTheTabSteps_Fails(decimal speed)
    {
        var device = GuitarTabDevice
            .Create(GuitarTabDeviceId.NewId(), UserId, "Phone", 0.2m, 0, false, CreatedOn)
            .Value;

        var result = device.UpdateSettings(speed, 0, false, CreatedOn.AddMinutes(5));

        result.IsFailure.Should().BeTrue();
        device.Speed.Should().Be(0.2m);
    }
}
