using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Contracts.Dtos;

public sealed record CreatedPersonalAccessTokenDto(
    PersonalAccessTokenId Id,
    string Name,
    string Token,
    string TokenPrefix,
    DateTime CreatedOnUtc,
    DateTime ExpiresAt
);
