using System.Net;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Contracts.Integration.Common;

public sealed record AddCookiesRequest(List<Cookie> Cookies);

public sealed record AddCookiesResponse(bool IsSuccess);
