using Quartz;
using Serilog;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.ShutdownJobs;

internal sealed class SaveFoodApiCookiesJob(
    ILogger logger,
    FoodApiCookieContainer foodApiCookieContainer,
    IFoodApiCookiesManager foodApiCookiesManager
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.Information("Saving FoodApi cookies to database.");

        var cookies = foodApiCookieContainer.GetAllCookies().ToList();

        var result = await foodApiCookiesManager.AddCookiesToDbAsync(
            cookies,
            CancellationToken.None
        );

        if (result.IsFailure)
        {
            logger.Error(
                "Failed to save FoodApi cookies to database. Error: {Error}",
                result.Error
            );
            return;
        }

        logger.Information(
            "FoodApi cookies saved to database. Cookies count: {CookiesCount}",
            cookies.Count
        );
    }
}
