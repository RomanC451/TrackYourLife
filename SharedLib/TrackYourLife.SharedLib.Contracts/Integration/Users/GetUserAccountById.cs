using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

/// <summary>
/// Request account contact data by user id (e.g. for YouTube settings password reset email).
/// </summary>
public sealed record GetUserAccountByIdRequest(UserId UserId);

public sealed record UserAccountDto(string Email, DateTime? VerifiedOnUtc);

public sealed record GetUserAccountByIdResponse(UserAccountDto? Data, List<Error> Errors);
