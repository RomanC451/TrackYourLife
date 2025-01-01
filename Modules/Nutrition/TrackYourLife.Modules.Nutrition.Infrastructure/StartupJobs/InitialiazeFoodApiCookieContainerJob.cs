using System.Net;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.StartupJobs;

public class InitializeFoodApiCookieContainerJob(
    ILogger logger,
    FoodApiCookieContainer cookieContainer,
    IFoodApiCookiesManager foodApiCookiesManager
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("Initializing FoodApi cookie container.");

        var cookies = await foodApiCookiesManager.GetCookiesFromDbAsync();

        cookies.ForEach(cookie => cookieContainer.Add(cookie));

        logger.Information(
            "FoodApi cookie container initialized. Cookies count: {CookiesCount}",
            cookies.Count
        );
    }
}