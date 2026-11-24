using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Contracts.Dtos;

public sealed record PersonalAccessTokenDto(
    PersonalAccessTokenId Id,
    string Name,
    string TokenPrefix,
    DateTime CreatedOnUtc,
    DateTime? LastUsedOnUtc,
    DateTime ExpiresAt
);
