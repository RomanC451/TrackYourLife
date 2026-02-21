using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;

public sealed record SetStripeCustomerIdCommand(UserId UserId, string StripeCustomerId)
    : ICommand;
