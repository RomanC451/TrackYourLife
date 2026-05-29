using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.FunctionalTests.Mocks;

internal sealed class SuccessfulEmailService : IEmailService
{
    public Result SendVerificationEmail(string userEmail, string verificationLink) =>
        Result.Success();

    public Result SendYoutubeSettingsPasswordResetEmail(string userEmail, string newPassword) =>
        Result.Success();
}
