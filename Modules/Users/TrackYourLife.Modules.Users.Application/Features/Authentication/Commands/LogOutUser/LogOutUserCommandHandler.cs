using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;

internal sealed class LogOutUserCommandHandler(
    IAuthService authService,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<LogOutUserCommand>
{
    public async Task<Result> Handle(LogOutUserCommand command, CancellationToken cancellationToken)
    {
        await authService.LogOutUserAsync(
            userIdentifierProvider.UserId,
            command.DeviceId,
            command.LogOutAllDevices,
            cancellationToken
        );

        return Result.Success();
    }
}
