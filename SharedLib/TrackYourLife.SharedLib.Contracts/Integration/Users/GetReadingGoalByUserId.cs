using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users;

public sealed record GetReadingGoalByUserIdRequest(UserId UserId, DateOnly Date);

public sealed record GetReadingGoalByUserIdResponse(int? TargetPages, List<Error> Errors);
