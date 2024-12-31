using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Common;

public sealed record IdResponse(IStronglyTypedGuid Id);
