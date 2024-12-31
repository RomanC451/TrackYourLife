using System.Net;

namespace TrackYourLife.SharedLib.Contracts.Integration.Common;

public sealed record GetCookiesByDomainsRequest(string[] Domains);

public sealed record GetCookiesByDomainsResponse(List<Cookie> Cookies);
