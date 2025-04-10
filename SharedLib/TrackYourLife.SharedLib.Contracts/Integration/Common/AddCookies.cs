using System.Net;

namespace TrackYourLife.SharedLib.Contracts.Integration.Common;

public sealed record AddCookiesRequest(List<Cookie> Cookies);

public sealed record AddCookiesResponse(bool IsSuccess);
