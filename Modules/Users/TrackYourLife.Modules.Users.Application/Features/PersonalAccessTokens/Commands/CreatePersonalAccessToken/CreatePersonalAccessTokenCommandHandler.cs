using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.CreatePersonalAccessToken;

internal sealed class CreatePersonalAccessTokenCommandHandler(
    IPersonalAccessTokenRepository personalAccessTokenRepository,
    IPersonalAccessTokenHasher personalAccessTokenHasher,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<CreatePersonalAccessTokenCommand, CreatedPersonalAccessTokenDto>
{
    public async Task<Result<CreatedPersonalAccessTokenDto>> Handle(
        CreatePersonalAccessTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var rawToken = personalAccessTokenHasher.GenerateRawToken();
        var tokenHash = personalAccessTokenHasher.Hash(rawToken);
        var tokenPrefix = personalAccessTokenHasher.GetVisiblePrefix(rawToken);
        var createdOnUtc = DateTime.UtcNow;
        var expiresAt = createdOnUtc.Add(PersonalAccessToken.DefaultLifetime);

        var createResult = PersonalAccessToken.Create(
            PersonalAccessTokenId.NewId(),
            userIdentifierProvider.UserId,
            command.Name,
            tokenPrefix,
            tokenHash,
            createdOnUtc,
            expiresAt
        );

        if (createResult.IsFailure)
        {
            return Result.Failure<CreatedPersonalAccessTokenDto>(createResult.Error);
        }

        await personalAccessTokenRepository.AddAsync(createResult.Value, cancellationToken);

        return Result.Success(
            new CreatedPersonalAccessTokenDto(
                createResult.Value.Id,
                createResult.Value.Name,
                rawToken,
                createResult.Value.TokenPrefix,
                createResult.Value.CreatedOnUtc,
                createResult.Value.ExpiresAt
            )
        );
    }
}
