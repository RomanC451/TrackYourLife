using MediatR;
using Microsoft.FeatureManagement;
using TrackYourLife.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Common.Domain.Repositories;
using TrackYourLife.Common.Domain.Users.DomainEvents;
using TrackYourLife.Common.Domain.Users.Repositories;

namespace TrackYourLife.Modules.Users.Application.Users.Events;

internal sealed class UserRegisteredDomainEventHandler
    : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    private readonly IFeatureManager _featureManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserRegisteredDomainEventHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        IAuthService authService,
        IFeatureManager featureManager,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _authService = authService;
        _featureManager = featureManager;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UserRegisteredDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(notification.UserId, cancellationToken);

        if (user is null)
        {
            return;
        }

        if (await _featureManager.IsEnabledAsync(FeatureFlags.SkipEmailVerification))
        {
            return;
        }

        string emailVerificationLink = await _authService.GenerateEmailVerificationLinkAsync(
            user.Id,
            cancellationToken
        );

        _emailService.SendVerificationEmail(user.Email, emailVerificationLink);
    }
}
