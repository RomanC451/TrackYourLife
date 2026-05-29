using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;

public sealed record VerifyYoutubeSettingsPasswordCommand(string Password) : ICommand;
