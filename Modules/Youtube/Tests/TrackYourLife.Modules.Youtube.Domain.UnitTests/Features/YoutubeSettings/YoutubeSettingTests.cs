using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Domain.UnitTests.Features.YoutubeSettings;

public class YoutubeSettingTests
{
    private readonly YoutubeSettingsId _id = YoutubeSettingsId.NewId();
    private readonly UserId _userId = UserId.NewId();
    private readonly DateTime _createdOnUtc = DateTime.UtcNow;

    [Fact]
    public void Create_WithoutPassword_ShouldSucceed()
    {
        var result = YoutubeSetting.Create(_id, _userId, null, _createdOnUtc);

        result.IsSuccess.Should().BeTrue();
        result.Value.HasPassword.Should().BeFalse();
        result.Value.SettingsPasswordHash.Should().BeNull();
    }

    [Fact]
    public void Create_WithPasswordHash_ShouldSucceed()
    {
        var result = YoutubeSetting.Create(_id, _userId, "hash;value", _createdOnUtc);

        result.IsSuccess.Should().BeTrue();
        result.Value.HasPassword.Should().BeTrue();
    }

    [Fact]
    public void SetPasswordHash_ShouldUpdateHash()
    {
        var setting = YoutubeSetting.Create(_id, _userId, null, _createdOnUtc).Value;
        var utc = DateTime.UtcNow;

        var result = setting.SetPasswordHash("new-hash", utc);

        result.IsSuccess.Should().BeTrue();
        setting.SettingsPasswordHash.Should().Be("new-hash");
        setting.ModifiedOnUtc.Should().Be(utc);
    }

    [Fact]
    public void ClearPassword_ShouldRemoveHash()
    {
        var setting = YoutubeSetting.Create(_id, _userId, "hash;value", _createdOnUtc).Value;

        var result = setting.ClearPassword(DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
        setting.HasPassword.Should().BeFalse();
    }

    [Fact]
    public void CanRequestPasswordResetEmail_WhenNoPassword_ShouldFail()
    {
        var setting = YoutubeSetting.Create(_id, _userId, null, _createdOnUtc).Value;

        var result = setting.CanRequestPasswordResetEmail(
            DateTime.UtcNow,
            YoutubeSetting.PasswordResetEmailCooldown
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.PasswordNotSet);
    }

    [Fact]
    public void CanRequestPasswordResetEmail_WithinCooldown_ShouldFail()
    {
        var setting = YoutubeSetting.Create(_id, _userId, "hash", _createdOnUtc).Value;
        var sentAt = new DateTime(2026, 5, 21, 12, 0, 0, DateTimeKind.Utc);
        setting.RecordPasswordResetEmailSent(sentAt);

        var result = setting.CanRequestPasswordResetEmail(
            sentAt.AddMinutes(2),
            YoutubeSetting.PasswordResetEmailCooldown
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(YoutubeSettingsErrors.ResetEmailRateLimited);
    }

    [Fact]
    public void CanRequestPasswordResetEmail_AfterCooldown_ShouldSucceed()
    {
        var setting = YoutubeSetting.Create(_id, _userId, "hash", _createdOnUtc).Value;
        var sentAt = new DateTime(2026, 5, 21, 12, 0, 0, DateTimeKind.Utc);
        setting.RecordPasswordResetEmailSent(sentAt);

        var result = setting.CanRequestPasswordResetEmail(
            sentAt.AddMinutes(6),
            YoutubeSetting.PasswordResetEmailCooldown
        );

        result.IsSuccess.Should().BeTrue();
    }
}
