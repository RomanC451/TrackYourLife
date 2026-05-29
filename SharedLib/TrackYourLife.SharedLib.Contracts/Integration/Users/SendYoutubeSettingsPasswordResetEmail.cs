using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

public sealed record SendYoutubeSettingsPasswordResetEmailRequest(
    string Email,
    string NewPassword
);

public sealed record SendYoutubeSettingsPasswordResetEmailResponse(List<Error> Errors);
