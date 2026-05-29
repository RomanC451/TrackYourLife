namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;

internal sealed record VerifyYoutubeSettingsPasswordRequest(string Password);

internal sealed record SetYoutubeSettingsPasswordRequest(
    string? CurrentPassword,
    string? NewPassword
);
