using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.SetYoutubeSettingsPassword;

internal sealed class SetYoutubeSettingsPasswordCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IYoutubeSettingsPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<SetYoutubeSettingsPasswordCommand>
{
    public async Task<Result> Handle(
        SetYoutubeSettingsPasswordCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var utcNow = dateTimeProvider.UtcNow;

        var existingSettings = await youtubeSettingsRepository.GetByUserIdAsync(
            userId,
            cancellationToken
        );

        if (string.IsNullOrEmpty(request.NewPassword))
        {
            return RemovePasswordAsync(existingSettings, request.CurrentPassword, utcNow);
        }

        var newPasswordResult = YoutubeSettingsPassword.Create(request.NewPassword);
        if (newPasswordResult.IsFailure)
        {
            return Result.Failure(newPasswordResult.Error);
        }

        if (existingSettings is null || !existingSettings.HasPassword)
        {
            return await SetInitialPasswordAsync(
                existingSettings,
                userId,
                newPasswordResult.Value.Value,
                utcNow,
                cancellationToken
            );
        }

        return ChangePasswordAsync(
            existingSettings,
            request.CurrentPassword,
            newPasswordResult.Value.Value,
            utcNow
        );
    }

    private async Task<Result> SetInitialPasswordAsync(
        YoutubeSetting? existingSettings,
        UserId userId,
        string plainPassword,
        DateTime utcNow,
        CancellationToken cancellationToken
    )
    {
        if (existingSettings is not null && existingSettings.HasPassword)
        {
            return Result.Failure(YoutubeSettingsErrors.PasswordAlreadySet);
        }

        var hash = passwordHasher.Hash(plainPassword);

        if (existingSettings is null)
        {
            var createResult = YoutubeSetting.Create(
                YoutubeSettingsId.NewId(),
                userId,
                hash,
                utcNow
            );

            if (createResult.IsFailure)
            {
                return Result.Failure(createResult.Error);
            }

            await youtubeSettingsRepository.AddAsync(createResult.Value, cancellationToken);
            return Result.Success();
        }

        var setResult = existingSettings.SetPasswordHash(hash, utcNow);
        if (setResult.IsFailure)
        {
            return setResult;
        }

        youtubeSettingsRepository.Update(existingSettings);
        return Result.Success();
    }

    private Result ChangePasswordAsync(
        YoutubeSetting settings,
        string? currentPassword,
        string newPlainPassword,
        DateTime utcNow
    )
    {
        if (string.IsNullOrEmpty(currentPassword))
        {
            return Result.Failure(YoutubeSettingsErrors.CurrentPasswordRequired);
        }

        if (
            settings.SettingsPasswordHash is null
            || !passwordHasher.Verify(settings.SettingsPasswordHash, currentPassword)
        )
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidPassword);
        }

        var hash = passwordHasher.Hash(newPlainPassword);
        var updateResult = settings.SetPasswordHash(hash, utcNow);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        youtubeSettingsRepository.Update(settings);
        return Result.Success();
    }

    private Result RemovePasswordAsync(
        YoutubeSetting? settings,
        string? currentPassword,
        DateTime utcNow
    )
    {
        if (settings is null || !settings.HasPassword)
        {
            return Result.Failure(YoutubeSettingsErrors.PasswordNotSet);
        }

        if (string.IsNullOrEmpty(currentPassword))
        {
            return Result.Failure(YoutubeSettingsErrors.CurrentPasswordRequired);
        }

        if (
            settings.SettingsPasswordHash is null
            || !passwordHasher.Verify(settings.SettingsPasswordHash, currentPassword)
        )
        {
            return Result.Failure(YoutubeSettingsErrors.InvalidPassword);
        }

        var clearResult = settings.ClearPassword(utcNow);
        if (clearResult.IsFailure)
        {
            return clearResult;
        }

        youtubeSettingsRepository.Update(settings);
        return Result.Success();
    }
}
