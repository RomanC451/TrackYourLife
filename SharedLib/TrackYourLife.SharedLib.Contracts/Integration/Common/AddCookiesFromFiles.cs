using System.Net;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.SharedLib.Contracts.Integration.Common;

public sealed record AddCookiesFromFilesRequest(byte[] CookieFile);

public sealed record AddCookiesFromFilesResponse(List<Cookie>? Data, Error Error);
