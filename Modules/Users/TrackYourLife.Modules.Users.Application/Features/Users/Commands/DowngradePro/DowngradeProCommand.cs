using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;

public sealed record DowngradeProCommand(UserId UserId, SubscriptionStatus SubscriptionStatus) : ICommand;
