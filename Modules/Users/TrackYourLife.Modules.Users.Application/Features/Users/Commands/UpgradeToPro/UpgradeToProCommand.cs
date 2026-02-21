using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;

public sealed record UpgradeToProCommand(
    UserId UserId,
    string StripeCustomerId,
    DateTime PeriodEndUtc,
    bool CancelAtPeriodEnd = false
) : ICommand;
