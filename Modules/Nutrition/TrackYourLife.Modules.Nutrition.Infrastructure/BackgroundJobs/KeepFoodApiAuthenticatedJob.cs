using Quartz;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class KeepFoodApiAuthenticatedJob(IFoodApiService foodApiService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await foodApiService.SearchFoodAndAddToDbAsync("lapte", CancellationToken.None);
    }
}
