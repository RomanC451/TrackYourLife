using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Presentation.Features.PersonalAccessTokens;

internal static class PersonalAccessTokensMappingsExtensions
{
    public static PersonalAccessTokenDto ToDto(this PersonalAccessTokenReadModel token) =>
        new(
            token.Id,
            token.Name,
            token.TokenPrefix,
            token.CreatedOnUtc,
            token.LastUsedOnUtc,
            token.ExpiresAt
        );
}
