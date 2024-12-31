using MediatR;
using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.Modules.Users.Domain.Users.DomainEvents;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Events;

internal sealed class UserRegisteredDomainEventHandler(
    IUserQuery userQuery,
    IEmailService emailService,
    IAuthService authService,
    UsersFeatureManagement featureManager
) : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var user = await userQuery.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        if (featureManager.SkipEmailVerification)
        {
            return;
        }

        var linkGeneratorResult = await authService.GenerateEmailVerificationLinkAsync(
            user.Id,
            cancellationToken
        );

        if (linkGeneratorResult.IsFailure)
        {
            return;
        }

        var emailVerificationLink = linkGeneratorResult.Value;

        emailService.SendVerificationEmail(user.Email, emailVerificationLink);
    }
}
