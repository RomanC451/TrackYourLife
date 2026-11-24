using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

public sealed record PersonalAccessTokenReadModel(
    PersonalAccessTokenId Id,
    UserId UserId,
    string Name,
    string TokenPrefix,
    DateTime CreatedOnUtc,
    DateTime? LastUsedOnUtc,
    DateTime ExpiresAt,
    DateTime? RevokedOnUtc
) : IReadModel<PersonalAccessTokenId>;
