using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;

public sealed record UpdateProSubscriptionPeriodEndCommand(
    UserId UserId,
    DateTime PeriodEndUtc,
    SubscriptionStatus SubscriptionStatus,
    bool CancelAtPeriodEnd = false
) : ICommand;
