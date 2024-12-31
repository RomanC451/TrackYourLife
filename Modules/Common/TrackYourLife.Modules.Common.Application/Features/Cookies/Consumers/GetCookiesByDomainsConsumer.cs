using MassTransit;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;

namespace TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;

public sealed class GetCookiesByDomainsConsumer(ICookieQuery cookieQuery)
    : IConsumer<GetCookiesByDomainsRequest>
{
    public async Task Consume(ConsumeContext<GetCookiesByDomainsRequest> context)
    {
        List<CookieReadModel> cookies = [];

        foreach (var domain in context.Message.Domains)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                continue;
            }

            cookies.AddRange(
                await cookieQuery.GetCookiesByDomainAsync(domain, context.CancellationToken)
            );
        }

        var systemCookies = cookies
            .Select(x => new System.Net.Cookie(x.Name, x.Value, x.Path, x.Domain))
            .ToList();

        await context.RespondAsync(new GetCookiesByDomainsResponse(systemCookies));
    }
}
