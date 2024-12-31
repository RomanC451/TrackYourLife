using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;

namespace TrackYourLife.Modules.Common.Application.Features.Cookies.Consumers;

public sealed class AddCookiesConsumer(
    ICookieRepository cookieRepository,
    ILogger logger,
    ICommonUnitOfWork unitOfWork
) : IConsumer<AddCookiesRequest>
{
    public async Task Consume(ConsumeContext<AddCookiesRequest> context)
    {
        foreach (var cookie in context.Message.Cookies)
        {
            var existingCookie = await cookieRepository.GetByNameAndDomainAsync(
                cookie.Name,
                cookie.Domain,
                CancellationToken.None
            );

            if (existingCookie is not null)
            {
                existingCookie.UpdateValue(cookie.Value);
                cookieRepository.Update(existingCookie);
                continue;
            }

            var result = Cookie.Create(
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

            await cookieRepository.AddAsync(cookie: result.Value, CancellationToken.None);
        }

        await context.RespondAsync(new AddCookiesResponse(true));

        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
