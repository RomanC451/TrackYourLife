using MediatR;
using Serilog;
using TrackYourLife.Modules.Users.Application.Core;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Events;

internal sealed class UserRegisteredDomainEventHandler(
    IUserQuery userQuery,
    IEmailService emailService,
    IAuthService authService,
    UsersFeatureFlags featureManager,
    ILogger logger
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
            logger.Error(
                "Failed to handle UserRegisteredDomainEvent: User with id {UserId} not found",
                notification.UserId
            );
            throw new EventFailedException(
                $"User with id {notification.UserId} not found when handling UserRegisteredDomainEvent"
            );
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
            logger.Error(
                "Failed to handle UserRegisteredDomainEvent: Failed to generate email verification link for user {UserId}. Error: {Error}",
                user.Id,
                linkGeneratorResult.Error
            );
            throw new EventFailedException(
                $"Failed to generate email verification link for user {user.Id} when handling UserRegisteredDomainEvent. Error: {linkGeneratorResult.Error}"
            );
        }

        var emailVerificationLink = linkGeneratorResult.Value;

        var emailResult = emailService.SendVerificationEmail(user.Email, emailVerificationLink);

        if (emailResult.IsFailure)
        {
            logger.Error(
                "Failed to handle UserRegisteredDomainEvent: Failed to send verification email to {Email} for user {UserId}. Error: {Error}",
                user.Email,
                user.Id,
                emailResult.Error
            );
            throw new EventFailedException(
                $"Failed to send verification email to {user.Email} for user {user.Id} when handling UserRegisteredDomainEvent. Error: {emailResult.Error}"
            );
        }
    }
}
