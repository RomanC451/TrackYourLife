using MassTransit;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class SendYoutubeSettingsPasswordResetEmailConsumer(IEmailService emailService)
    : IConsumer<SendYoutubeSettingsPasswordResetEmailRequest>
{
    public async Task Consume(ConsumeContext<SendYoutubeSettingsPasswordResetEmailRequest> context)
    {
        var result = emailService.SendYoutubeSettingsPasswordResetEmail(
            context.Message.Email,
            context.Message.NewPassword
        );

        if (result.IsFailure)
        {
            await context.RespondAsync(
                new SendYoutubeSettingsPasswordResetEmailResponse([result.Error])
            );
            return;
        }

        await context.RespondAsync(new SendYoutubeSettingsPasswordResetEmailResponse([]));
    }
}
