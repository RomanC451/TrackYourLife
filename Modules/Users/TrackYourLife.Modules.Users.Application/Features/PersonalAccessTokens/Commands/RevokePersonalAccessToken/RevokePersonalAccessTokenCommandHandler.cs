using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;

internal sealed class RevokePersonalAccessTokenCommandHandler(
    IPersonalAccessTokenRepository personalAccessTokenRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<RevokePersonalAccessTokenCommand>
{
    public async Task<Result> Handle(
        RevokePersonalAccessTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var token = await personalAccessTokenRepository.GetByIdAsync(
            command.Id,
            cancellationToken
        );

        if (token is null)
        {
            return Result.Failure(PersonalAccessTokenErrors.NotFound(command.Id));
        }

        if (token.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(PersonalAccessTokenErrors.NotOwned(command.Id));
        }

        var revokeResult = token.Revoke(DateTime.UtcNow);

        if (revokeResult.IsFailure)
        {
            return revokeResult;
        }

        personalAccessTokenRepository.Update(token);

        return Result.Success();
    }
}
