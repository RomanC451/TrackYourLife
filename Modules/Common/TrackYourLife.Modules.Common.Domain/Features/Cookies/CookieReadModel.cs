using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Common.Domain.Features.Cookies;

public sealed record CookieReadModel(
    CookieId Id,
    string Name,
    string Value,
    string Domain,
    string Path
) : IReadModel<CookieId>;
