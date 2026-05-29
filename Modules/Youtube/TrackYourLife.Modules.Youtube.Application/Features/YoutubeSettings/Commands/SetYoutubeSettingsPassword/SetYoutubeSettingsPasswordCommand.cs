using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;

public sealed record SetYoutubeSettingsPasswordCommand(
    string? CurrentPassword,
    string? NewPassword
) : ICommand;
