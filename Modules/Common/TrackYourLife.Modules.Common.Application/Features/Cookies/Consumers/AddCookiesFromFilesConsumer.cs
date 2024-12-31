using MassTransit;
using Serilog;
using TrackYourLife.Modules.Common.Application.Core.Abstractions.Services;
using TrackYourLife.Modules.Common.Domain.Core;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Contracts.Integration.Common;
using TrackYourLife.SharedLib.Domain.Errors;

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
            CancellationToken.None
        );

        if (cookiesResult.IsFailure || cookiesResult.Value.Count == 0)
        {
            await context.RespondAsync(
                new AddCookiesFromFilesErrorResponse(
                    InfrastructureErrors.CookieReader.FailedToReadCookies
                )
            );
            return;
        }

        foreach (var cookie in cookiesResult.Value)
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

            await cookieRepository.AddAsync(result.Value, CancellationToken.None);
        }

        await context.RespondAsync(new AddCookiesFromFilesResponse(cookiesResult.Value));

        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
