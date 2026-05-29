using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.VerifyYoutubeSettingsPassword;

internal sealed class VerifyYoutubeSettingsPasswordCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IYoutubeSettingsPasswordHasher passwordHasher
) : ICommandHandler<VerifyYoutubeSettingsPasswordCommand>
{
    public async Task<Result> Handle(
        VerifyYoutubeSettingsPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var settings = await youtubeSettingsRepository.GetByUserIdAsync(userId, cancellationToken);

        if (settings is null || !settings.HasPassword)
        {
            return Result.Failure(YoutubeSettingsErrors.PasswordNotSet);
        }

        if (
            settings.SettingsPasswordHash is null
            || !passwordHasher.Verify(settings.SettingsPasswordHash, request.Password)
        )
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidPassword);
        }

        return Result.Success();
    }
}
