using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using Cookie = System.Net.Cookie;

namespace TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;

public sealed class AddCookiesFromFilesConsumer(
    ICookiesReader cookieReader,
    ICookieRepository cookieRepository,
    ILogger logger,
    ICommonUnitOfWork unitOfWork
) : IConsumer<AddCookiesFromFilesRequest>
{
    public async Task Consume(ConsumeContext<AddCookiesFromFilesRequest> context)
    {
        var cookiesResult = await cookieReader.GetCookiesAsync(
            context.Message.CookieFile,
            context.CancellationToken
        );

        if (cookiesResult.IsFailure || cookiesResult.Value.Count == 0)
        {
            await context.RespondAsync(
                new AddCookiesFromFilesResponse(
                    null,
                    InfrastructureErrors.CookieReader.FailedToReadCookies
                )
            );
            return;
        }

        // Use a dictionary to track unique cookies by name and domain
        var uniqueCookies = new Dictionary<(string Name, string Domain), Cookie>();

        foreach (var cookie in cookiesResult.Value)
        {
            var key = (cookie.Name, cookie.Domain);
            uniqueCookies[key] = cookie;
        }

        var domainCookies = new List<Domain.Features.Cookies.Cookie>();

        foreach (var cookie in uniqueCookies.Values)
        {
            var existingCookie = await cookieRepository.GetByNameAndDomainAsync(
                cookie.Name,
                cookie.Domain,
                context.CancellationToken
            );

            if (existingCookie is not null)
            {
                existingCookie.UpdateValue(cookie.Value);
                cookieRepository.Update(existingCookie);
                domainCookies.Add(existingCookie);
                continue;
            }

            var result = Domain.Features.Cookies.Cookie.Create(
                CookieId.NewId(),
                cookie.Name,
                cookie.Value,
                cookie.Domain,
                cookie.Path
            );

            if (result.IsFailure)
            {
                logger.Error(
                    "Failed to create a new cookie with name {Name} and domain {Domain}. Error: {Error}",
                    cookie.Name,
                    cookie.Domain,
                    result.Error
                );
                continue;
            }

            await cookieRepository.AddAsync(result.Value, context.CancellationToken);
            domainCookies.Add(result.Value);
        }

        var systemCookies = domainCookies
            .Select(c => new Cookie(c.Name, c.Value, c.Path, c.Domain))
            .ToList();

        await unitOfWork.SaveChangesAsync(context.CancellationToken);

        await context.RespondAsync(new AddCookiesFromFilesResponse(systemCookies, Error.None));
    }
}
