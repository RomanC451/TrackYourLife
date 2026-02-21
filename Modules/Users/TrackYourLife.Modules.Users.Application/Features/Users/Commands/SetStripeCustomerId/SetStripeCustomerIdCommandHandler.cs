using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;

internal sealed class SetStripeCustomerIdCommandHandler(IUserRepository userRepository)
    : ICommandHandler<SetStripeCustomerIdCommand>
{
    public async Task<Result> Handle(
        SetStripeCustomerIdCommand command,
        CancellationToken cancellationToken
    )
    {
        User? user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(UserErrors.NotFound(command.UserId));
        }

        user.SetStripeCustomerId(command.StripeCustomerId);
        userRepository.Update(user);

        return Result.Success();
    }
}
