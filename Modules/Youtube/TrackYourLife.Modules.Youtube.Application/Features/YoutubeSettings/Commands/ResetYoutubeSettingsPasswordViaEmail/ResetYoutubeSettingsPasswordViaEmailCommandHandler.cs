using MassTransit;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;
using TrackYourLife.Modules.Youtube.Application.Core.Security;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.ResetYoutubeSettingsPasswordViaEmail;

internal sealed class ResetYoutubeSettingsPasswordViaEmailCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeSettingsRepository youtubeSettingsRepository,
    IYoutubeSettingsPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    IBus bus
) : ICommandHandler<ResetYoutubeSettingsPasswordViaEmailCommand>
{
    public async Task<Result> Handle(
        ResetYoutubeSettingsPasswordViaEmailCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var utcNow = dateTimeProvider.UtcNow;

        var settings = await youtubeSettingsRepository.GetByUserIdAsync(
            userId,
            cancellationToken
        );

        if (settings is null)
        {
            return Result.Failure(YoutubeSettingsErrors.PasswordNotSet);
        }

        var canResetResult = settings.CanRequestPasswordResetEmail(
            utcNow,
            YoutubeSetting.PasswordResetEmailCooldown
        );
        if (canResetResult.IsFailure)
        {
            return canResetResult;
        }

        var accountClient = bus.CreateRequestClient<GetUserAccountByIdRequest>();
        var accountResponse = await accountClient.GetResponse<GetUserAccountByIdResponse>(
            new GetUserAccountByIdRequest(userId),
            cancellationToken
        );

        if (accountResponse.Message.Errors.Count > 0)
        {
            return Result.Failure(accountResponse.Message.Errors[0]);
        }

        var account = accountResponse.Message.Data;
        if (account is null)
        {
            return Result.Failure(YoutubeSettingsErrors.UserNotFound);
        }

        if (account.VerifiedOnUtc is null)
        {
            return Result.Failure(YoutubeSettingsErrors.AccountEmailNotVerified);
        }

        var passwordResult = YoutubeSettingsPasswordGenerator.Generate();
        if (passwordResult.IsFailure)
        {
            return Result.Failure(passwordResult.Error);
        }

        var plainPassword = passwordResult.Value;
        var previousHash = settings.SettingsPasswordHash;
        var newHash = passwordHasher.Hash(plainPassword);

        var setHashResult = settings.SetPasswordHash(newHash, utcNow);
        if (setHashResult.IsFailure)
        {
            return setHashResult;
        }

        youtubeSettingsRepository.Update(settings);

        var emailClient = bus.CreateRequestClient<SendYoutubeSettingsPasswordResetEmailRequest>();
        var emailResponse =
            await emailClient.GetResponse<SendYoutubeSettingsPasswordResetEmailResponse>(
                new SendYoutubeSettingsPasswordResetEmailRequest(account.Email, plainPassword),
                cancellationToken
            );

        if (emailResponse.Message.Errors.Count > 0)
        {
            if (!string.IsNullOrEmpty(previousHash))
            {
                settings.SetPasswordHash(previousHash, utcNow);
                youtubeSettingsRepository.Update(settings);
            }

            return Result.Failure(YoutubeSettingsErrors.FailedToSendResetEmail);
        }

        var recordResult = settings.RecordPasswordResetEmailSent(utcNow);
        if (recordResult.IsFailure)
        {
            return recordResult;
        }

        youtubeSettingsRepository.Update(settings);
        return Result.Success();
    }
}
